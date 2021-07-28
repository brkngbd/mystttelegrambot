namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    public class YandexConnectionHelper
    {
        private readonly YandexConnectionConfig connectionConfig;
        readonly HttpClient client;

        public YandexConnectionHelper(IHttpClientFactory httpClientFactory, IOptions<YandexConnectionConfig> connectionConfig)
        {
            this.connectionConfig = connectionConfig.Value;
            this.client = httpClientFactory.CreateClient();
        }

        public async Task<string> GetIAMToken()
        {
            var values = new Dictionary<string, string>
            {
                { "yandexPassportOauthToken", connectionConfig.OAuthToken }
            };
            var jsonRequestBody = JsonSerializer.Serialize<Dictionary<string, string>>(values);

            var request = new HttpRequestMessage(HttpMethod.Post, connectionConfig.TokenApiEndpointUri);
            request.Content = new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(jsonResponse))
            {
                var token = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse)
                                        ["iamToken"]?.ToString();
                return token;
            }

            throw new Exception("Failed to get yandex cloud api iam token.");
        }

        public string GetSpeechApiUri(string lang)
        {
            return $"{connectionConfig.SpeechApiEndpointUri}?lang={lang}&folderId={connectionConfig.FolderId}";
        }
    }
}
