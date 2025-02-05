using MassTransit;
using Newtonsoft.Json;
using Orders.Infrastructure.Contexts;

namespace Orders.API.BackgroundServices
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var bus = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            _logger.LogInformation("Outbox Processor started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = dbContext.OutboxMessages
                    .Where(m => !m.Processed)
                    .ToList();

                foreach (var message in messages)
                {
                    var eventType = Type.GetType(message.EventType);
                    var eventInstance = JsonConvert.DeserializeObject(message.Payload, eventType);

                    await bus.Publish(eventInstance, stoppingToken);
                    message.Processed = true;
                }

                await dbContext.SaveChangesAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            _logger.LogInformation("Outbox Processor stopping");
        }
    }

}
