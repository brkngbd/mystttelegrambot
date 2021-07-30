namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    ///   The telegram message sender
    /// </summary>
    public class TelegramMessageSender
    {
        /// <summary>The connection helper</summary>
        private readonly TelegramAPIConnectionHelper connectionHelper;

        /// <summary>The HTTP client factory</summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>Initializes a new instance of the <see cref="TelegramMessageSender" /> class.</summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="connectionHelper">The connection helper.</param>
        public TelegramMessageSender(IHttpClientFactory httpClientFactory, TelegramAPIConnectionHelper connectionHelper)
        {
            this.connectionHelper = connectionHelper;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>Sends the message.</summary>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="messageText">The message text.</param>
        public async Task<string> SendMessage(string chatId, string messageText)
        {
            var client = this.httpClientFactory.CreateClient(Microsoft.Extensions.Options.Options.DefaultName);

            var url = this.connectionHelper.GetTelegramApiUri() + "/sendMessage";

            var values = new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", !string.IsNullOrWhiteSpace(messageText) ? messageText : "dont understand" }
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
