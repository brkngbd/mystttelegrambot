namespace VoiceRecognitionTelegramBotBackend
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public class SpeechToText
    {
        readonly YandexSpeechRecognizer stt;
        readonly TelegramFileDownloader tfd;
        readonly TelegramMessageSender tms;
        readonly HttpClient client;


        public SpeechToText(IHttpClientFactory httpClientFactory, YandexSpeechRecognizer stt, TelegramFileDownloader tfd, TelegramMessageSender tms)
        {
            this.stt = stt;
            this.tfd = tfd;
            this.tms = tms;

            this.client = httpClientFactory.CreateClient();
        }

        public async Task<string> RecognizeAndReply(string botMessage)
        {
            var parser = new RequestMessageParser();

            var parsedMessage = parser.Parse(botMessage);

            var file = await this.tfd.DowloadFile(parsedMessage.FileId);

            var text = await stt.SpeechToText(file);

            return await tms.SendMessage(parsedMessage.ChatId, text);
        }
     
    }
}
