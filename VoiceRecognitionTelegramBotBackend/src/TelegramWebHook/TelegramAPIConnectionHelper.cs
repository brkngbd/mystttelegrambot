namespace VoiceRecognitionTelegramBotBackend
{
    using Microsoft.Extensions.Options;

    /// <summary>
    ///   The Telegram API connection helper
    /// </summary>
    public class TelegramAPIConnectionHelper
    {
        /// <summary>The connection configuration</summary>
        private readonly TelegramConnectionConfig connectionConfig;

        /// <summary>Initializes a new instance of the <see cref="TelegramAPIConnectionHelper" /> class.</summary>
        /// <param name="connectionConfig">The connection configuration.</param>
        public TelegramAPIConnectionHelper(IOptions<TelegramConnectionConfig> connectionConfig)
        {
            this.connectionConfig = connectionConfig.Value;
        }

        /// <summary>Gets the telegram API URI.</summary>
        public string GetTelegramApiUri()
        {
            return $"{connectionConfig.APIEndPointUri}/bot{connectionConfig.APIToken}";
        }

        /// <summary>Gets the telegram file download API URI.</summary>
        public string GetTelegramFileDownloadApiUri()
        {
            return $"{connectionConfig.APIEndPointUri}/file/bot{connectionConfig.APIToken}";
        }
    }
}
