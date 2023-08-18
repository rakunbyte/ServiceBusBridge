using Confluent.Kafka;

var config = new ProducerConfig {
    BootstrapServers = "localhost:29092",
    SecurityProtocol = SecurityProtocol.Plaintext,
};

using (var p = new ProducerBuilder<Null, string>(config).Build())
{
    while (true)
    {
        for(var i = 0; i < 100; i++)
            try
            {
                var dr = await p.ProduceAsync("topic1", new Message<Null, string> { Value=$"TEST MESSAGE {i}" });
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        
        for(var i = 200; i < 300; i++)
            try
            {
                var dr = await p.ProduceAsync("topic2", new Message<Null, string> { Value=$"TEST MESSAGE {i}" });
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        
    }
    
}