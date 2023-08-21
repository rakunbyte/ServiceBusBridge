using BackingServices.Common;
using Confluent.Kafka;
using KafkaBridge.Configuration;
using Serilog;

namespace KafkaBridge.ConsumerServices;

public interface IKafkaConsumerService : IConsumerService
{
    
}

public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly KafkaConsumerServiceConfig Config;
    private readonly IBackingService BackingService;
    
    public KafkaConsumerService(KafkaConsumerServiceConfig config, IBackingService backingService)
    {
        Config = config;
        BackingService = backingService;
    }
    
    public async Task StartConsumer(CancellationToken cancellationToken)
    {
        Log.Information("Starting {Service} for {@KafkaConsumerServiceConfig}", nameof(KafkaConsumerService), Config);
        var consumerConfig = Config.CreateConsumerConfig();
        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        
        consumer.Subscribe(Config.Topic);
        try
        {
            while (true)
            {
                var consumeResult = consumer.Consume(cancellationToken);
                Log.Information("Kafka Message received, sending to backing service. Message: {@Message}", consumeResult.Message.Value);
                await BackingService.ProcessMessage(consumeResult.Message.Value);
            }
        }
        catch (Exception e) {
            Log.Error(e, "There was an Exception in the Kafka Client");
        }
        finally{
            Log.Warning("Kafka Client is shutting down!");

            consumer.Close();
            consumer.Dispose();
        }
    }
}