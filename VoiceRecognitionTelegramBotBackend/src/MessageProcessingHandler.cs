namespace VoiceRecognitionTelegramBotBackend
{
    using System.Threading.Tasks;

    /// <summary>
    ///   Used to parse Telegram WebHook message and initialize voice recognition process
    /// </summary>
    public class MessageProcessingHandler
    {
        /// <summary>The service bus queue message sender</summary>
        private readonly ServiceBusSender sender;

        /// <summary>Initializes a new instance of the <see cref="MessageProcessingHandler" /> class.</summary>
        /// <param name="sender">The sender.</param>
        public MessageProcessingHandler(ServiceBusSender sender)
        {
            this.sender = sender;
        }

        /// <summary>Initializes the processing.</summary>
        /// <param name="botMessage">The bot message.</param>
        public async Task InitializeProcessing(string botMessage)
        {
            var parser = new RequestMessageParser();
            var parsedMessage = parser.Parse(botMessage);
            await this.sender.SendMessage(parsedMessage);
        }
    }
}
