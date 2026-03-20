
using Azure.Messaging.ServiceBus;
using CommandService.Data;
using CommandService.Models;
using Microservices.Common.Models;
using Microservices.Common.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace CommandService.Services
{
    public class ServiceBusMessageReceiver : IMessageReceiver
    {
        private static readonly ActivitySource ActivityConfig = new ActivitySource("servicebusmessagereceiver");

        // The Service Bus client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when messages are being published or read
        // regularly.
        // the client that owns the connection and can be used to create senders and receivers
        private ServiceBusClient _client;

        // the sender used to publish messages to the queue
        //one sender per message type? N0.
        //Create ServiceBusSender once per queue
        private ServiceBusProcessor _processor;
        private readonly IServiceScope _scope;
        private readonly CommandServiceContext _commandServiceContext;
        private readonly ILogger<ServiceBusMessageReceiver> _logger;

        public ServiceBusMessageReceiver(IConfiguration configuration, IServiceProvider serviceProvider,
            ILogger<ServiceBusMessageReceiver> logger)
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
            // set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
            // If you use the default AmqpTcp, you will need to make sure that the ports 5671 and 5672 are open
            var clientOptions = new ServiceBusClientOptions();
            _client = new ServiceBusClient(connectionString, clientOptions);
            _processor = _client.CreateProcessor(queueName);
            this._scope = serviceProvider.CreateScope();
            this._commandServiceContext = this._scope.ServiceProvider.GetRequiredService<CommandServiceContext>();
            this._logger = logger;
        }

        // handle received messages
        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            args.Message.ApplicationProperties.TryGetValue("trace-id", out var producerTraceId);
            args.Message.ApplicationProperties.TryGetValue("span-id", out var producerSpanId);

            var producerActivityContext = new ActivityContext(ActivityTraceId.CreateFromString(producerTraceId?.ToString()),
   ActivitySpanId.CreateFromString(producerSpanId?.ToString()),
   ActivityTraceFlags.Recorded);

            using (var currentActivity = ActivityConfig.StartActivity("Consume Message", ActivityKind.Consumer,
               producerActivityContext
            ))
            {
                string body = args.Message.Body.ToString();
                var platform = JsonSerializer.Deserialize<PlatformMessageModel>(body);
                this._logger.LogInformation("Platform information received. TraceId: {TraceId}, Message: {Message}", producerTraceId, body);
                if (platform != null)
                {
                    var existingPlatform = await _commandServiceContext.Platforms.FirstOrDefaultAsync(p => p.Id == platform.Id);
                    if (existingPlatform != null)
                    {
                        existingPlatform.Name = platform.Name;
                        existingPlatform.Publisher = platform.Publisher;
                        _commandServiceContext.Platforms.Update(existingPlatform);
                    }
                    else
                    {
                        _commandServiceContext.Platforms.Add(new Platform
                        {
                            Id = platform.Id,
                            Name = platform.Name,
                            Publisher = platform.Publisher
                        });
                    }
                    await _commandServiceContext.SaveChangesAsync();
                }
                // complete the message. message is deleted from the queue. 
                await args.CompleteMessageAsync(args.Message);
            }
        }

        // handle any errors when receiving messages
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            this._logger.LogError(args.Exception, "Error processing message: {ErrorSource}", args.ErrorSource);
            return Task.CompletedTask;
        }

        public async Task StartProcessingAsync(CancellationToken cancellationToken)
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync(cancellationToken);
        }

        public void Dispose()
        {
            this._processor.DisposeAsync().GetAwaiter().GetResult();
            this._client.DisposeAsync().GetAwaiter().GetResult();
            this._scope.Dispose();
        }
    }
}
