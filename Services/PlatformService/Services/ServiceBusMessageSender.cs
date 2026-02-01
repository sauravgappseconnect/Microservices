using Azure.Messaging.ServiceBus;
using Microservices.Common.Services;

namespace PlatformService.Services
{
    public class ServiceBusMessageSender : IMessageSender
    {
        private string _connectionString;
        private string _queueName;

        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.
        // the client that owns the connection and can be used to create senders and receivers
        private ServiceBusClient _client;

        // the sender used to publish messages to the queue
        //one sender per message type? N0.
        //Create ServiceBusSender once per queue
        private ServiceBusSender _sender;
        public ServiceBusMessageSender(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("ServiceBus:ConnectionString")?.Value;
            var queueName = configuration.GetSection("ServiceBus:QueueName")?.Value;
            if (connectionString == null)
            {
                throw new ArgumentNullException("ServiceBus connection string is null");
            }
            if (queueName == null)
            {
                throw new ArgumentException("ServiceBus queueName is null");
            }
            this._connectionString = connectionString;
            this._queueName = queueName;
            // set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
            // If you use the default AmqpTcp, you will need to make sure that the ports 5671 and 5672 are open
            var clientOptions = new ServiceBusClientOptions();
            _client = new ServiceBusClient(connectionString, clientOptions);
            _sender = _client.CreateSender(queueName);
        }

        public async Task SendMessageAsync(string message)
        {
            await _sender.SendMessageAsync(new ServiceBusMessage(message));
        }
    }
}
