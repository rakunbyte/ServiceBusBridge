using BackingServices.Common;
using KafkaBridge.Configuration;
using KafkaBridge.ConsumerServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KafkaBridge;

public class MainService : IHostedService
{
    private IEnumerable<IBackingService> BackingServices;
    private IEnumerable<KafkaConsumerServiceConfig> KafkaConsumerServiceConfigs;
    private IEnumerable<AzureServiceBusConsumerServiceConfig> AzureServiceBusConsumerServiceConfigs;


    private List<Task> KafkaConsumerTasks = new();
    private List<Task> AzureServiceBusConsumerTasks = new();
    
    public MainService(
        IEnumerable<IBackingService> backingServices,
        IOptions<KafkaConsumerServiceConfigs> kconfigs,
        IOptions<AzureServiceBusConsumerServiceConfigs> aconfigs
        )
    {
        BackingServices = backingServices;
        KafkaConsumerServiceConfigs = kconfigs.Value.Configs;
        AzureServiceBusConsumerServiceConfigs = aconfigs.Value.Configs;

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
        foreach (var config in KafkaConsumerServiceConfigs)
        {
            var backingService = BackingServices.First(x => x.Name == config.BackingServiceName);
            var consumer = new KafkaConsumerService(config, backingService);
            KafkaConsumerTasks.Add(Task.Run(() => consumer.StartConsumer(cancellationToken)));
        }
    }
    
    private async Task StartAzureServiceBusConsumers(CancellationToken cancellationToken)
    {
        foreach (var config in AzureServiceBusConsumerServiceConfigs)
        {
            var backingService = BackingServices.First(x => x.Name == config.BackingServiceName);
            var consumer = new AzureServiceBusConsumerService();
            AzureServiceBusConsumerTasks.Add(Task.Run(() => consumer.StartConsumer(cancellationToken)));
        }
    }
}