namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    ///   The Telegram file downloader
    /// </summary>
    public class TelegramFileDownloader
    {
        /// <summary>The connection helper</summary>
        private readonly TelegramAPIConnectionHelper connectionHelper;

        /// <summary>The client</summary>
        private readonly HttpClient client;

        /// <summary>Initializes a new instance of the <see cref="TelegramFileDownloader" /> class.</summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="connectionHelper">The connection helper.</param>
        public TelegramFileDownloader(IHttpClientFactory httpClientFactory, TelegramAPIConnectionHelper connectionHelper)
        {
            this.connectionHelper = connectionHelper;
            this.client = httpClientFactory.CreateClient();
        }

        /// <summary>Downloads the file.</summary>
        /// <param name="fileId">The file identifier.</param>
        public async Task<byte[]> DownloadFile(string fileId)
        {
            var fileInfo = await this.GetFileInfo(fileId);
            if (fileInfo.TryGetValue("file_path", out var filePathObject) && filePathObject != null)
            {
                var url = $"{connectionHelper.GetTelegramFileDownloadApiUri()}/{filePathObject}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                HttpResponseMessage response = await this.client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var byteResponse = await response.Content.ReadAsByteArrayAsync();
                if (byteResponse != null)
                {
                    return byteResponse;
                }
            }

            throw new Exception("Failed to get voice file info");
        }

        /// <summary>Gets the file information.</summary>
        /// <param name="fileId">The file identifier.</param>
        private async Task<Dictionary<string, object>> GetFileInfo(string fileId)
        {
            var url = this.connectionHelper.GetTelegramApiUri() + $"/getFile?file_id={fileId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await this.client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(jsonResponse))
            {
                var data =
                    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);

                var result =
                    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(data["result"].ToString());

                return result;
            }

            throw new Exception("Failed to get voice file info");
        }
    }
}
