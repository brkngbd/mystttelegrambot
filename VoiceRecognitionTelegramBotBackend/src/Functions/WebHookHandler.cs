namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class WebHookHandler
    {
        readonly MessageProcessingHandler speechToText;

        public WebHookHandler(MessageProcessingHandler speechToText)
        {
            this.speechToText = speechToText;
        }

        [FunctionName("WebHookHandler")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string botToken = req.Query["bottoken"];
            if ("secretkey2021" != botToken)
            {
                return new UnauthorizedResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            log.LogInformation($"Request body: {requestBody}.");

            try
            {
                await this.speechToText.InitializeProcessing(requestBody);
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
