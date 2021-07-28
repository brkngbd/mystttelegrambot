namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class YandexSpeechRecognizer
    {
        private readonly YandexConnectionHelper tokenHelper;
        readonly HttpClient client;

        public YandexSpeechRecognizer(IHttpClientFactory httpClientFactory, YandexConnectionHelper tokenHelper, string lang)
        {
            this.tokenHelper = tokenHelper;
            this.client = httpClientFactory.CreateClient();
        }

        public async Task<string> SpeechToText(byte[] voiceData)
        {
            var url = tokenHelper.GetSpeechApiUri("ru-RU");
            var token = await tokenHelper.GetIAMToken();

            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new ByteArrayContent(voiceData);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(jsonResponse))
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);
                if (dict != null && dict.ContainsKey("result"))
                {
                    return dict["result"]?.ToString();
                }
            }

            throw new Exception("Failed to convert speech to text");
        }
    }
}
