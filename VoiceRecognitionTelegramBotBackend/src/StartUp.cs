[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup(typeof(VoiceRecognitionTelegramBotBackend.Startup))]

namespace VoiceRecognitionTelegramBotBackend
{
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///   Used to tune up the dependency injection
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>Configures the specified builder.</summary>
        /// <param name="builder">The builder.</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

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
    }
}