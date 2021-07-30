namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    ///   Telegram WebHook handler
    /// </summary>
    public class WebHookHandler
    {
        /// <summary>The message processing handler</summary>
        private readonly MessageProcessingHandler messageProcessingHandler;

        /// <summary>Initializes a new instance of the <see cref="WebHookHandler" /> class.</summary>
        /// <param name="speechToText">The speech to text.</param>
        public WebHookHandler(MessageProcessingHandler speechToText)
        {
            this.messageProcessingHandler = speechToText;
        }

        /// <summary>Runs the specified request.</summary>
        /// <param name="request">The request.</param>
        /// <param name="log">The log.</param>
        [FunctionName("WebHookHandler")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string botToken = request.Query["bottoken"];
            if (botToken != "secretkey2021")
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            log.LogInformation($"Request body: {requestBody}.");

            try
            {
                await this.messageProcessingHandler.InitializeProcessing(requestBody);
            }
            catch (Exception exc)
            {
                log.LogError(exc.ToString());
            }

            string responseMessage = $"This HTTP triggered function executed successfully. Request body: {requestBody}";

            return new OkObjectResult(responseMessage);
        }
    }
}
