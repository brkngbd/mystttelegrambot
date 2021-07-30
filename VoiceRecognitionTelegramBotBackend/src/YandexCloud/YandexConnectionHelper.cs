namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    /// <summary>
    ///   The Yandex Cloud API connection helper
    /// </summary>
    public class YandexConnectionHelper
    {
        /// <summary>The connection configuration</summary>
        private readonly YandexConnectionConfig connectionConfig;

        /// <summary>The HTTP client factory</summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>Initializes a new instance of the <see cref="YandexConnectionHelper" /> class.</summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="connectionConfig">The connection configuration.</param>
        public YandexConnectionHelper(IHttpClientFactory httpClientFactory, IOptions<YandexConnectionConfig> connectionConfig)
        {
            this.connectionConfig = connectionConfig.Value;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>Gets the iam token.</summary>
        public async Task<string> GetIAMToken()
        {
            var client = httpClientFactory.CreateClient(Microsoft.Extensions.Options.Options.DefaultName);

            var values = new Dictionary<string, string>
            {
                { "yandexPassportOauthToken", this.connectionConfig.OAuthToken }
            };
            var jsonRequestBody = JsonSerializer.Serialize<Dictionary<string, string>>(values);

            var request = new HttpRequestMessage(HttpMethod.Post, this.connectionConfig.TokenApiEndpointUri);
            request.Content = new StringContent(jsonRequestBody, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(jsonResponse))
            {
                var token = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse)["iamToken"]?.ToString();
                return token;
            }

            throw new Exception("Failed to get yandex cloud api iam token.");
        }

        /// <summary>Gets the speech API URI.</summary>
        /// <param name="lang">The language.</param>
        public string GetSpeechApiUri(string lang)
        {
            return $"{connectionConfig.SpeechApiEndpointUri}?lang={lang}&folderId={connectionConfig.FolderId}";
        }
    }
}
