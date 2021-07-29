namespace VoiceRecognitionTelegramBotBackend
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Microsoft.Extensions.Options;

    /// <summary>
    ///   The ServiceBus queue message sender
    /// </summary>
    public class ServiceBusSender
    {
        /// <summary>The client</summary>
        private readonly ServiceBusClient client;

        /// <summary>The client sender</summary>
        private readonly Azure.Messaging.ServiceBus.ServiceBusSender clientSender;

        /// <summary>Initializes a new instance of the <see cref="ServiceBusSender" /> class.</summary>
        /// <param name="configuration">The configuration.</param>
        public ServiceBusSender(IOptions<ServiceBusConfig> configuration)
        {
            var connectionString = configuration.Value.Connection;
            this.client = new ServiceBusClient(connectionString);
            this.clientSender = this.client.CreateSender(configuration.Value.QueueName);
        }

        /// <summary>Sends the message.</summary>
        /// <param name="payload">The payload.</param>
        public async Task SendMessage<T>(T payload)
        {
            string messagePayload = JsonSerializer.Serialize(payload);
            ServiceBusMessage message = new ServiceBusMessage(messagePayload);
            await this.clientSender.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}