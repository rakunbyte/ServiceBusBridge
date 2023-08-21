using BackingServices.Common;
using KafkaBridge.Configuration;
using KafkaBridge.Consumers;

namespace KafkaBridge;

public class MainService
{
    private IEnumerable<IBackingService> BackingServices;

    private List<Task> KafkaConsumerTasks = new();
    private List<Task> AzureServiceBusConsumerTasks = new();
    
    public MainService(IEnumerable<IBackingService> backingServices)
    {
        BackingServices = backingServices;
    }

    public async Task Start(IEnumerable<KafkaConsumerServiceConfig> kafkaConsumerServiceConfigs, IEnumerable<AzureServiceBusConsumerServiceConfig> azureServiceBusConsumerServiceConfigs)
    {
        await StartKafkaConsumers(kafkaConsumerServiceConfigs);
        await StartAzureServiceBusConsumers(azureServiceBusConsumerServiceConfigs);
    }

    private async Task StartKafkaConsumers(IEnumerable<KafkaConsumerServiceConfig> kafkaConsumerServiceConfigs)
    {
        foreach (var config in kafkaConsumerServiceConfigs)
        {
            var backingService = BackingServices.First(x => x.Name == config.BackingServiceName);
            var consumer = new KafkaConsumerService(config, backingService);
            KafkaConsumerTasks.Add(Task.Run(() => consumer.StartConsumer()));
        }
    }
    
    private async Task StartAzureServiceBusConsumers(IEnumerable<AzureServiceBusConsumerServiceConfig> azureServiceBusConsumerServiceConfigs)
    {
        foreach (var config in azureServiceBusConsumerServiceConfigs)
        {
            var backingService = BackingServices.First(x => x.Name == config.BackingServiceName);
            var consumer = new AzureServiceBusConsumerService();
            AzureServiceBusConsumerTasks.Add(Task.Run(() => consumer.StartConsumer()));
        }
    }
}