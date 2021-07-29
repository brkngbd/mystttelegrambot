namespace VoiceRecognitionTelegramBotBackend
{
    /// <summary>
    ///   The parsed Telegram WebHook message
    /// </summary>
    public class WebHookMessage
    {
        /// <summary>Gets or sets the file identifier.</summary>
        /// <value>The file identifier.</value>
        public string FileId { get; set; }

        /// <summary>Gets or sets the chat identifier.</summary>
        /// <value>The chat identifier.</value>
        public string ChatId { get; set; }
    }
}
