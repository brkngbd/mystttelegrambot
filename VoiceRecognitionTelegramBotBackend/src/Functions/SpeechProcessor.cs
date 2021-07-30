namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///   The speech processing handler
    /// </summary>
    public class SpeechProcessor
    {
        /// <summary>The Yandex speech to text</summary>
        private readonly YandexSpeechRecognizer yandexSpeechToText;

        /// <summary>The telegram file downloader</summary>
        private readonly TelegramFileDownloader telegramFileDownloader;

        /// <summary>The telegram message sender</summary>
        private readonly TelegramMessageSender telegramMessageSender;

        /// <summary>Initializes a new instance of the <see cref="SpeechProcessor" /> class.</summary>
        /// <param name="yandexSpeechToText">The Yandex speech to text.</param>
        /// <param name="telegramFileDownloader">The Telegram file downloader.</param>
        /// <param name="telegramMessageSender">The telegram message sender.</param>
        public SpeechProcessor(YandexSpeechRecognizer yandexSpeechToText, TelegramFileDownloader telegramFileDownloader, TelegramMessageSender telegramMessageSender)
        {
            this.yandexSpeechToText = yandexSpeechToText;
            this.telegramFileDownloader = telegramFileDownloader;
            this.telegramMessageSender = telegramMessageSender;
        }

        /// <summary>Runs the orchestrator.</summary>
        /// <param name="context">The context.</param>
        [FunctionName("OrchestrationMain")]
        public async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = string.Empty;

            var parameters = context.GetInput<WebHookMessage>();

            byte[] file = await context.CallActivityAsync<byte[]>("OrchestrationActivity_DownloadFile", parameters.FileId);
            if (file != null)
            {
                string text = await context.CallActivityAsync<string>("OrchestrationActivity_ConvertToText", file);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    outputs = await context.CallActivityAsync<string>("OrchestrationActivity_SendBackToChat", (parameters.ChatId, text));
                }
            }

            return outputs;
        }

        /// <summary>Downloads the file.</summary>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="log">The log.</param>
        [FunctionName("OrchestrationActivity_DownloadFile")]
        public async Task<byte[]> DownloadFile([ActivityTrigger] string fileId, ILogger log)
        {
            byte[] result = null;
            try
            {
                log.LogInformation($"Download file from Telegram. File id: {fileId}.");
                result = await this.telegramFileDownloader.DownloadFile(fileId);
            }
            catch (Exception e)
            {
                log.LogInformation($"Failed convert voice message to text. '{e.Message}'.");
            }

            return result;
        }

        /// <summary>Converts to text.</summary>
        /// <param name="fileData">The file data.</param>
        /// <param name="log">The log.</param>
        [FunctionName("OrchestrationActivity_ConvertToText")]
        public async Task<string> ConvertToText([ActivityTrigger] byte[] fileData, ILogger log)
        {
            string result = string.Empty;
            try
            {
                log.LogInformation($"Convert ogg audio data to text. Binary size: {fileData.Length}.");
                result = await this.yandexSpeechToText.SpeechToText(fileData);
            }
            catch (Exception e)
            {
                log.LogInformation($"Failed convert voice message to text. '{e.Message}'.");
            }

            return result;
        }

        /// <summary>Sends the back to chat.</summary>
        /// <param name="messageInfo">The message information.</param>
        /// <param name="log">The log.</param>
        [FunctionName("OrchestrationActivity_SendBackToChat")]
        public async Task<string> SendBackToChat([ActivityTrigger] (string, string) messageInfo, ILogger log)
        {
            string result = string.Empty;
            try
            {
                log.LogInformation($"Sending text '{messageInfo.Item2}' to Telegram chat with id '{messageInfo.Item1}'.");
                result = await this.telegramMessageSender.SendMessage(messageInfo.Item1, messageInfo.Item2);
            }
            catch (Exception e)
            {
                log.LogInformation($"Failed to send a message back to chat. '{e.Message}'.");
            }

            return result;
        }

        /// <summary>Queues the start.</summary>
        /// <param name="myQueueItem">My queue item.</param>
        /// <param name="starter">The starter.</param>
        /// <param name="log">The log.</param>
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
                    string instanceId = await starter.StartNewAsync<WebHookMessage>("OrchestrationMain", telegramMessageData);

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