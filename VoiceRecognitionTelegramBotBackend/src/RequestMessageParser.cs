namespace VoiceRecognitionTelegramBotBackend
{
    using System.Collections.Generic;
    using System;

    public class RequestMessageParser
    {
        public WebHookMessage Parse(string rawMessage)
        {
            var messageObject = GetMessage(rawMessage);
            if (messageObject != null)
            {
                var chatObject = messageObject.GetValueFromDictionary<Dictionary<string, object>>("chat");
                if (chatObject == null)
                {
                    throw new Exception("Failed to convert speech to text");
                }

                var chatId = chatObject.GetStringFromDictionary("id");

                var voiceObject = messageObject.GetValueFromDictionary<Dictionary<string, object>>("voice");
                if (voiceObject == null)
                {
                    throw new Exception("Failed to convert speech to text");
                }

                var mime_type = voiceObject.GetStringFromDictionary("mime_type");
                if (mime_type != "audio/ogg")
                {
                    throw new Exception("Failed to convert speech to text");
                }

                var file_id = voiceObject.GetStringFromDictionary("file_id");
                if (string.IsNullOrWhiteSpace(file_id))
                {
                    throw new Exception("Failed to convert speech to text");
                }

                return new WebHookMessage { ChatId = chatId, FileId = file_id };
            }

            throw new Exception("Failed to convert speech to text");
        }

        private Dictionary<string, object> GetMessage(string botMessage)
        {
            var body = botMessage.ToObject<Dictionary<string, object>>();
            if (body.TryGetValue("message", out var messageItem) && messageItem != null)
            {
                var messageObject =
                    System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(messageItem.ToString());

                return messageObject;
            }

            throw new Exception("Failed to convert speech to text");
        }
    }
}
