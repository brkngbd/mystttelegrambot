namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class TelegramMessageSender
    {
        private readonly TelegramAPIConnectionHelper connectionHelper;
        readonly HttpClient client;

        public TelegramMessageSender(IHttpClientFactory httpClientFactory, TelegramAPIConnectionHelper connectionHelper)
        {
            this.connectionHelper = connectionHelper;
            this.client = httpClientFactory.CreateClient();
        }

        public async Task<string> SendMessage(string chatId, string messageText)
        {
            var url = this.connectionHelper.GetTelegramApiUri() + "/sendMessage";

            var values = new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", !string.IsNullOrWhiteSpace(messageText)?messageText:"dont understand" }
            };
            var jsonBody = JsonSerializer.Serialize<Dictionary<string, string>>(values);

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(jsonResponse))
            {
                return jsonResponse;
            }

            throw new Exception("Failed to send reply to the chat");
        }
    }
}
