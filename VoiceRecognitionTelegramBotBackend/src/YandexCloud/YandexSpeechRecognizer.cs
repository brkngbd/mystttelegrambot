namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    ///   The speech to text convertor
    /// </summary>
    public class YandexSpeechRecognizer
    {
        /// <summary>The token helper</summary>
        private readonly YandexConnectionHelper tokenHelper;

        /// <summary>The client</summary>
        private readonly HttpClient client;

        /// <summary>Initializes a new instance of the <see cref="YandexSpeechRecognizer" /> class.</summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="tokenHelper">The token helper.</param>
        /// <param name="lang">The language.</param>
        public YandexSpeechRecognizer(IHttpClientFactory httpClientFactory, YandexConnectionHelper tokenHelper, string lang)
        {
            this.tokenHelper = tokenHelper;
            this.client = httpClientFactory.CreateClient();
        }

        /// <summary>Speeches to text.</summary>
        /// <param name="voiceData">The voice data.</param>
        public async Task<string> SpeechToText(byte[] voiceData)
        {
            var url = this.tokenHelper.GetSpeechApiUri("ru-RU");
            var token = await this.tokenHelper.GetIAMToken();

            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new ByteArrayContent(voiceData);

            HttpResponseMessage response = await this.client.SendAsync(request);
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
