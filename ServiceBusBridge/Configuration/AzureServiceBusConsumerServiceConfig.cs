namespace KafkaBridge.Configuration;


public class AzureServiceBusConsumerServiceConfigs
{
    public IEnumerable<AzureServiceBusConsumerServiceConfig> Configs { get; set; } = new List<AzureServiceBusConsumerServiceConfig>();
}

public class AzureServiceBusConsumerServiceConfig : IConsumerServiceConfig
{
    public string? BackingServiceName { get; set; }
    public string? ConnectionString { get; set; }
    public string? Topic { get; set; }
    public string? Subscription { get; set; }
}