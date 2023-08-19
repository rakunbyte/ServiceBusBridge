// See https://aka.ms/new-console-template for more information

using BackingServices;
using Confluent.Kafka;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

var BootStrapServers = "localhost:29092";
var GroupId = "groupId1";

var config = new ConsumerConfig
{
    BootstrapServers = BootStrapServers,
    GroupId = GroupId,
    AutoOffsetReset = AutoOffsetReset.Earliest
};

var backingService = new AccountCreatedService();
        
using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
{
    consumer.Subscribe("topic1");

    try
    {
        while (true)
        {
            //_logger.LogInformation($"GETTING HUNGRY");
            var consumeResult = consumer.Consume();
            await backingService.DoWork(consumeResult.Message.Value);

            //_logger.LogInformation($"Consumed event from topic topic1 with key {consumeResult.Message.Key,-10} and value {consumeResult.Message.Value}");
        }
    }
    catch (Exception e) {
        Console.WriteLine(e.Message);
        //_logger.LogError("KAFKA CONSUMER BLEW UP! " + e);
    }
    finally{
        //_logger.LogError("CONSUMER IS CLOSING!!!");
        consumer.Close();
    }
}