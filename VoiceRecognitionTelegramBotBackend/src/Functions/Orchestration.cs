using System;
namespace VoiceRecognitionTelegramBotBackend.src.Functions
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    public class Orchestration
    {
        readonly YandexSpeechRecognizer stt;
        readonly TelegramFileDownloader tfd;
        readonly TelegramMessageSender tms;

        public Orchestration(YandexSpeechRecognizer stt, TelegramFileDownloader tfd, TelegramMessageSender tms)
        {
            this.stt = stt;
            this.tfd = tfd;
            this.tms = tms;
        }

        [FunctionName("OrchestrationClient")]
        public async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = string.Empty;

            var parameters = context.GetInput<WebHookMessage>();

            byte[] file = await context.CallActivityAsync<byte[]>("OrchestrationClient_DowloadFile", parameters.FileId);
            string text = await context.CallActivityAsync<string>("OrchestrationClient_ConvertToText", file);
            outputs = await context.CallActivityAsync<string>("OrchestrationClient_SendBackToChat", (parameters.ChatId, text));

            return outputs;
        }

        [FunctionName("OrchestrationClient_DowloadFile")]
        public async Task<byte[]> DowloadFile([ActivityTrigger] string fileId, ILogger log)
        {
            log.LogInformation($"Download file from Telegram. File id: {fileId}.");
            return await this.tfd.DowloadFile(fileId);
        }

        [FunctionName("OrchestrationClient_ConvertToText")]
        public async Task<string> ConvertToText([ActivityTrigger] byte[] fileData, ILogger log)
        {
            log.LogInformation($"Convert ogg audio data to text. Binary size: {fileData.Length}.");
            return await stt.SpeechToText(fileData);
        }

        [FunctionName("OrchestrationClient_SendBackToChat")]
        public async Task<string> SendBackToChat([ActivityTrigger] (string, string) messageInfo, ILogger log)
        {
            log.LogInformation($"Sending text '{messageInfo.Item2}' to Telegram chat with id '{messageInfo.Item1}'.");
            return await tms.SendMessage(messageInfo.Item1, messageInfo.Item2);
        }

        [FunctionName("OrchestrationClient_QueueStart")]
        public async Task QueueStart(
            [ServiceBusTrigger("queue1", Connection = "ServiceBusConfig:Connection")] string myQueueItem,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            try
            {
                var telegramMessageData = myQueueItem.ToObject<WebHookMessage>();
                if (telegramMessageData != null)
                {
                    string instanceId = await starter.StartNewAsync<WebHookMessage>("OrchestrationClient", telegramMessageData);

                    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
                    return;
                }
                else
                {
                    log.LogInformation($"Unexpected message received = '{myQueueItem}'.");
                }
            }
            catch (Exception e)
            {
                log.LogInformation($"Failed deserialize message {myQueueItem}. '{e.Message}'.");
            }
        }
    }
}