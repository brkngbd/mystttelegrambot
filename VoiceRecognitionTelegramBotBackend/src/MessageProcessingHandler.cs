namespace VoiceRecognitionTelegramBotBackend
{
    using System.Threading.Tasks;

    public class MessageProcessingHandler
    {
        readonly ServiceBusSender sender;

        public MessageProcessingHandler(ServiceBusSender sender)
        {
            this.sender = sender;
        }

        public async Task InitializeProcessing(string botMessage)
        {
            var parser = new RequestMessageParser();
            var parsedMessage = parser.Parse(botMessage);
            await sender.SendMessage(parsedMessage);
        }
    }
}
