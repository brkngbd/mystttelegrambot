namespace VoiceRecognitionTelegramBotBackend
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Microsoft.Extensions.Options;

    public class ServiceBusSender
    {
        private readonly ServiceBusClient _client;
        private readonly Azure.Messaging.ServiceBus.ServiceBusSender _clientSender;

        public ServiceBusSender(IOptions<ServiceBusConfig> configuration)
        {
            var connectionString = configuration.Value.Connection;
            _client = new ServiceBusClient(connectionString);
            _clientSender = _client.CreateSender(configuration.Value.QueueName);
        }

        public async Task SendMessage<T>(T payload)
        {
            string messagePayload = JsonSerializer.Serialize(payload);
            ServiceBusMessage message = new ServiceBusMessage(messagePayload);
            await _clientSender.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}