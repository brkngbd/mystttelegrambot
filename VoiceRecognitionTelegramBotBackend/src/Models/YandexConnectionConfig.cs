namespace VoiceRecognitionTelegramBotBackend
{
    /// <summary>
    ///   The YandexCloud API parameters
    /// </summary>
    public class YandexConnectionConfig 
    {
        /// <summary>Gets or sets the o authentication token.</summary>
        /// <value>The authentication token.</value>
        public string OAuthToken { get; set;  }

        /// <summary>Gets or sets the folder identifier.</summary>
        /// <value>The folder identifier.</value>
        public string FolderId { get; set;  }

        /// <summary>Gets or sets the token API endpoint URI.</summary>
        /// <value>The token API endpoint URI.</value>
        public string TokenApiEndpointUri { get; set;  }

        /// <summary>Gets or sets the speech API endpoint URI.</summary>
        /// <value>The speech API endpoint URI.</value>
        public string SpeechApiEndpointUri { get; set;  }
    }
}
