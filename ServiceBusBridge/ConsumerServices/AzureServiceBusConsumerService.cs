using Serilog;

namespace KafkaBridge.Consumers;

public interface IAzureServiceBusConsumerService : IConsumerService
{
}

public class AzureServiceBusConsumerService : IAzureServiceBusConsumerService
{
    public Task StartConsumer()
    {
        Log.Information("Azure Service Bus consuming");
        return Task.CompletedTask;
    }
}