using BackingServices;
using Confluent.Kafka;

namespace KafkaBridge.Configuration;

public class KafkaConsumerServiceConfig : IConsumerServiceConfig
{
    public string? BackingServiceName { get; set; }
    public string? Topic { get; set; }
    public string? BootstrapServers { get; set; }
    public string? GroupId { get; set; }
    public ConsumerConfig CreateConsumerConfig()
    {
        return new ConsumerConfig
        {
            BootstrapServers = BootstrapServers,
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }
}