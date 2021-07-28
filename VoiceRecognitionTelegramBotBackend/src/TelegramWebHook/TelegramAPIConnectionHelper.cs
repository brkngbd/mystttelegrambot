namespace VoiceRecognitionTelegramBotBackend
{
    using Microsoft.Extensions.Options;

    public class TelegramAPIConnectionHelper
    {
        private readonly TelegramConnectionConfig connectionConfig;

        public TelegramAPIConnectionHelper(IOptions<TelegramConnectionConfig> connectionConfig)
        {
            this.connectionConfig = connectionConfig.Value;
        }

        public string GetTelegramApiUri()
        {
            return $"{connectionConfig.APIEndPointUri}/bot{connectionConfig.APIToken}";
        }

        public string GetTelegramFileDownloadApiUri()
        {
            return $"{connectionConfig.APIEndPointUri}/file/bot{connectionConfig.APIToken}";
        }
    }
}
