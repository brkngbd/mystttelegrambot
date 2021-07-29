namespace VoiceRecognitionTelegramBotBackend
{
    /// <summary>
    ///   Service bus client configuration
    /// </summary>
    public class ServiceBusConfig
    {
        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
        public string Connection { get; set; }

        /// <summary>Gets or sets the name of the queue.</summary>
        /// <value>The name of the queue.</value>
        public string QueueName { get; set; }
    }
}
