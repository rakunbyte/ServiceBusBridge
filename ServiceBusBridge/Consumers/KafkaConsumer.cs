using BackingServices.Common;
using Confluent.Kafka;
using KafkaBridge.Configuration;
using Serilog;

namespace KafkaBridge.Consumers;

public class KafkaConsumer
{
    private readonly KafkaConsumerConfig KafkaConsumerConfig;
    private readonly IBackingService BackingService;
    
    public KafkaConsumer(KafkaConsumerConfig kafkaConsumerConfig, IBackingService backingService)
    {
        KafkaConsumerConfig = kafkaConsumerConfig;
        BackingService = backingService;
    }
    
    public async Task Consume()
    {
        Log.Information("Starting KafkaConsumer for {@KafkaConsumerConfig}", KafkaConsumerConfig);
        var consumerConfig = KafkaConsumerConfig.CreateConsumerConfig();
        using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
        {
            consumer.Subscribe(KafkaConsumerConfig.Topic);
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
}