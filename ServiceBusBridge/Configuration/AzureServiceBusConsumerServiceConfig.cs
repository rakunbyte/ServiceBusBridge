namespace KafkaBridge.Configuration;

public class AzureServiceBusConsumerServiceConfig : IConsumerServiceConfig
{
    public string? BackingServiceName { get; set; }
}