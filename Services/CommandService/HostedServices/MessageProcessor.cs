
using Microservices.Common.Services;

namespace CommandService.HostedServices
{
    public class MessageProcessor : BackgroundService
    {
        private readonly IMessageReceiver messageReceiver;

        public MessageProcessor(IMessageReceiver messageReceiver)
        {
            this.messageReceiver = messageReceiver;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return messageReceiver.StartProcessingAsync(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            // Add any cleanup logic here if needed
            this.messageReceiver.Dispose();
            return base.StopAsync(cancellationToken);
        }
    }
}
