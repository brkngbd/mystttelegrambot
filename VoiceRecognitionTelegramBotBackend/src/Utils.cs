namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;

    public static class JsonExtensions
    {
        /// <summary>
        /// Serialization options
        /// </summary>
        private static readonly JsonSerializerOptions SerializerOptions =
            new JsonSerializerOptions()
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

        /// <summary>
        /// Method is used to deserialize the json string to the specified type.
        /// </summary>
        /// <param name="text">The string.</param>
        /// <typeparam name="T">The return value type.</typeparam>
        public static T ToObject<T>(this string text)
        {
            return JsonSerializer.Deserialize<T>(text, SerializerOptions);
        }

        /// <summary>
        /// Method is used to obtain a value from the dictionary or throw an exception if the specified value is missing or null.
        /// </summary>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key of the element.</param>
        /// <typeparam name="T">The return value type.</typeparam>
        public static T GetValueFromDictionary<T>(this Dictionary<string, object> dictionary, string key)
        {
            if (!(dictionary.TryGetValue(key, out var value) && value != null))
            {
                throw new IndexOutOfRangeException($"'{key}' property is missing or null.");
            }

            return value.ToString().ToObject<T>();
        }

        public static string GetStringFromDictionary(this Dictionary<string, object> dictionary, string key)
        {
            if (!(dictionary.TryGetValue(key, out var value) && value != null))
            {
                throw new IndexOutOfRangeException($"'{key}' property is missing or null.");
            }

            return value.ToString();
        }
    }
}
