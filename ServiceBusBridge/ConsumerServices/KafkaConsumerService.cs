using BackingServices.Common;
using Confluent.Kafka;
using KafkaBridge.Configuration;
using Serilog;

namespace KafkaBridge.Consumers;

public interface IKafkaConsumerService : IConsumerService
{
    
}

public class KafkaConsumerService : IConsumerService
{
    private readonly KafkaConsumerServiceConfig _kafkaConsumerServiceConfig;
    private readonly IBackingService BackingService;
    
    public KafkaConsumerService(KafkaConsumerServiceConfig kafkaConsumerServiceConfig, IBackingService backingService)
    {
        _kafkaConsumerServiceConfig = kafkaConsumerServiceConfig;
        BackingService = backingService;
    }
    
    public async Task StartConsumer()
    {
        Log.Information("Starting KafkaConsumerService for {@KafkaConsumerServiceConfig}", _kafkaConsumerServiceConfig);
        var consumerConfig = _kafkaConsumerServiceConfig.CreateConsumerConfig();
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        
        consumer.Subscribe(_kafkaConsumerServiceConfig.Topic);
        try
        {
            while (true)
            {
                var consumeResult = consumer.Consume();
                Log.Information("Message received, sending to backing service");
                await BackingService.ProcessMessage(consumeResult.Message.Value);
            }
        }
        catch (Exception e) {
            Log.Error(e, "There was an Exception in the Kafka Client");
        }
        finally{
            Log.Warning("Kafka Client is shutting down!");

            consumer.Close();
        }
    }
}