using BackingServices.Common;
using Destructurama;
using KafkaBridge.Configuration;
using KafkaBridge.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

Console.WriteLine("Starting Service Bus Consumer Host!");

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process);

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true);

var configuration = configurationBuilder.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Destructure.UsingAttributes()
    .Destructure.ToMaximumDepth(20)
    .CreateLogger();

var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(builder =>
{
    builder.AddSerilog(Log.Logger, true);
});
serviceCollection.AddBackingServices();

var serviceProvider = serviceCollection.BuildServiceProvider();
var backingServices = serviceProvider.GetServices<IBackingService>().ToList();

//For each Kafka Config, Create a Kafka Consumer
//For each ASB config, create an ASB Consumer
var kafkaConfigs = configuration.GetSection("KafkaConsumerConfigs").Get<List<KafkaConsumerConfig>>();
if (kafkaConfigs == null) throw new Exception("Balls");

var kafkaConsumerTasks = new List<Task>();
foreach (var config in kafkaConfigs)
{
    var backingService = backingServices.First(x => x.Name == config.BackingServiceName);
    var kafkaConsumer = new KafkaConsumer(config, backingService);
    kafkaConsumerTasks.Add(kafkaConsumer.Consume());
}

Console.ReadKey();

foreach (var task in kafkaConsumerTasks)
{
    task.Dispose();
}