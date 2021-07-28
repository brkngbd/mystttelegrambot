namespace VoiceRecognitionTelegramBotBackend
{
    public class YandexConnectionConfig 
    {
        public string OAuthToken { set; get; }
        public string FolderId { set; get; }
        public string TokenApiEndpointUri { set; get; }
        public string SpeechApiEndpointUri { set; get; }
    }
}
