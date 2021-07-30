[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup(typeof(VoiceRecognitionTelegramBotBackend.Startup))]

namespace VoiceRecognitionTelegramBotBackend
{
    using System;
    using System.Net.Http;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;
    using Polly.Extensions.Http;

    /// <summary>
    ///   Used to tune up the dependency injection
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>Configures the specified builder.</summary>
        /// <param name="builder">The builder.</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            builder.Services.AddOptions<YandexConnectionConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("YandexConnectionConfig").Bind(settings);
                });

            builder.Services.AddOptions<TelegramConnectionConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("TelegramConnectionConfig").Bind(settings);
                });

            builder.Services.AddOptions<ServiceBusConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("ServiceBusConfig").Bind(settings);
                });

            builder.Services.AddSingleton<ServiceBusSender>();
            builder.Services.AddSingleton<YandexConnectionHelper>();
            builder.Services.AddSingleton<YandexSpeechRecognizer>();
            builder.Services.AddSingleton<TelegramAPIConnectionHelper>();
            builder.Services.AddSingleton<TelegramFileDownloader>();
            builder.Services.AddSingleton<TelegramMessageSender>();
            builder.Services.AddTransient<MessageProcessingHandler>();
        }

        /// <summary>Gets the retry policy.</summary>
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}