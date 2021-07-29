namespace VoiceRecognitionTelegramBotBackend
{
    /// <summary>
    ///   The Telegram API parameters
    /// </summary>
    public class TelegramConnectionConfig
    {
        /// <summary>Gets or sets the API token.</summary>
        /// <value>The API token.</value>
        public string APIToken { get; set; }

        /// <summary>Gets or sets the API end point URI.</summary>
        /// <value>The API end point URI.</value>
        public string APIEndPointUri { get; set; }
    }
}
