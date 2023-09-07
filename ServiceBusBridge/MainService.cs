using System.Collections;
using BackingServices.Common;
using KafkaBridge.Configuration;
using KafkaBridge.ConsumerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KafkaBridge;

public class MainService : IHostedService
{
    private readonly IEnumerable<IKafkaConsumerService> KafkaConsumerServices;
    private readonly IEnumerable<IAzureServiceBusConsumerService> AzureServiceBusConsumerServices;


    private List<Task> KafkaConsumerTasks = new();
    private List<Task> AzureServiceBusConsumerTasks = new();
    
    public MainService(
        IEnumerable<IKafkaConsumerService> kafkaConsumerServices,
        IEnumerable<IAzureServiceBusConsumerService> azureServiceBusConsumerServices)
    {
        KafkaConsumerServices = kafkaConsumerServices;
        AzureServiceBusConsumerServices = azureServiceBusConsumerServices;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartKafkaConsumers(cancellationToken);
        await StartAzureServiceBusConsumers(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        //TODO what should happen on shutdown
    }
    
    private async Task StartKafkaConsumers(CancellationToken cancellationToken)
    {
        foreach (var consumer in KafkaConsumerServices)
        {
            KafkaConsumerTasks.Add(Task.Run(() => consumer.StartConsumer(cancellationToken), cancellationToken));
        }
    }
    
    private async Task StartAzureServiceBusConsumers(CancellationToken cancellationToken)
    {
        foreach (var consumer in AzureServiceBusConsumerServices)
        {
            AzureServiceBusConsumerTasks.Add(Task.Run(() => consumer.StartConsumer(cancellationToken), cancellationToken));
        }
    }
}