using Serilog;

namespace KafkaBridge.ConsumerServices;

public interface IAzureServiceBusConsumerService : IConsumerService
{
}

public class AzureServiceBusConsumerService : IAzureServiceBusConsumerService
{
    public Task StartConsumer(CancellationToken cancellationToken)
    {
        Log.Information("Azure Service Bus consuming");
        return Task.CompletedTask;
    }
}