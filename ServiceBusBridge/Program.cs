using BackingServices.Common;
using Destructurama;
using KafkaBridge;
using KafkaBridge.Configuration;
using KafkaBridge.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


//TODO use host pattern so we can return health checks instead

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
serviceCollection.AddScoped<MainService>();

var kafkaConsumerServiceConfigs = configuration.GetSection("KafkaConsumerConfigs").Get<List<KafkaConsumerServiceConfig>>()?? new List<KafkaConsumerServiceConfig>();;
var azureServiceBusConsumerServiceConfigs = configuration.GetSection("AzureServiceBusConsumerConfigs").Get<List<AzureServiceBusConsumerServiceConfig>>() ?? new List<AzureServiceBusConsumerServiceConfig>();

var serviceProvider = serviceCollection.BuildServiceProvider();
var mainService = serviceProvider.GetService<MainService>();

await mainService.Start(kafkaConsumerServiceConfigs, azureServiceBusConsumerServiceConfigs);
Console.ReadKey();
