namespace KafkaBridge.Configuration;


public class AzureServiceBusConsumerServiceConfigs
{
    public IEnumerable<AzureServiceBusConsumerServiceConfig> Configs { get; set; } = new List<AzureServiceBusConsumerServiceConfig>();
}

public class AzureServiceBusConsumerServiceConfig : IConsumerServiceConfig
{
    public string? BackingServiceName { get; set; }
}