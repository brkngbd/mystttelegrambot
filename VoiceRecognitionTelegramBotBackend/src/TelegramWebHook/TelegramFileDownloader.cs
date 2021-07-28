namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class TelegramFileDownloader
    {
        readonly TelegramAPIConnectionHelper connectionHelper;
        readonly HttpClient client;

        public TelegramFileDownloader(IHttpClientFactory httpClientFactory, TelegramAPIConnectionHelper connectionHelper)
        {
            this.connectionHelper = connectionHelper;
            this.client = httpClientFactory.CreateClient();
        }

        public async Task<byte[]> DowloadFile(string fileId)
        {
            var fileInfo = await GetFileInfo(fileId);
            if (fileInfo.TryGetValue("file_path", out var filePathObject) && filePathObject != null)
            {
                var url = $"{connectionHelper.GetTelegramFileDownloadApiUri()}/{filePathObject}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var byteResponse = await response.Content.ReadAsByteArrayAsync();
                if (byteResponse != null)
                {
                    return byteResponse;
                }
            }

            throw new Exception("Failed to get voice file info");
        }

        private async Task<Dictionary<string, object>> GetFileInfo(string fileId)
        {
            var url = this.connectionHelper.GetTelegramApiUri() + $"/getFile?file_id={fileId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await client.SendAsync(request);
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
