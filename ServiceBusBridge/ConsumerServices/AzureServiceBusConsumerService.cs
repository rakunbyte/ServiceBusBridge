using Azure.Messaging.ServiceBus;
using BackingServices.Common;
using KafkaBridge.Configuration;
using Serilog;

namespace KafkaBridge.ConsumerServices;

public interface IAzureServiceBusConsumerService : IConsumerService
{
}

public class AzureServiceBusConsumerService : IAzureServiceBusConsumerService
{
    private readonly AzureServiceBusConsumerServiceConfig Config;
    private readonly IBackingService BackingService;

    public AzureServiceBusConsumerService(AzureServiceBusConsumerServiceConfig config, IBackingService backingService)
    {
        Config = config;
        BackingService = backingService;
    }
    
    public async Task StartConsumer(CancellationToken cancellationToken)
    {
        Log.Information("Starting {Service} for {@AzureServiceBusConsumerServiceConfig}", nameof(AzureServiceBusConsumerService), Config);

        await using var client = new ServiceBusClient(Config.ConnectionString);
        var receiver = client.CreateReceiver(Config.Topic, Config.Subscription);

        try
        {
            while (true)
            {
                var message = await receiver.ReceiveMessageAsync(cancellationToken: cancellationToken);
                Log.Information("Azure Service Bus Message received, sending to backing service. Message: {@Message}", message.Body.ToString());
                await BackingService.ProcessMessage(message.Body.ToString());
            }
        }
        catch (Exception e) {
            Log.Error(e, "There was an Exception in the Azure Service Bus Client");
        }
        finally{
            Log.Warning("Azure Service Bus Client is shutting down!");

            await client.DisposeAsync();
        }
        
    }
}