using BackingServices.Common;
using Destructurama;
using KafkaBridge;
using KafkaBridge.Configuration;
using KafkaBridge.ConsumerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process);

//TODO probably better way of injecting config and logger
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

var host = CreateHostBuilder(args).Build();
await host.RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging((context, builder) =>
        {
            builder.AddSerilog(Log.Logger, true);
        })
        .ConfigureServices((builder, serviceCollection) =>
        {
            serviceCollection.AddBackingServices();

            var provider = serviceCollection.BuildServiceProvider();
            var backingServices = provider.GetServices<IBackingService>().ToList();
            
            //TODO hate wrapping this, figure out arrays
            var kafkaConsumerConfigs = builder.Configuration.GetSection("KafkaConsumerConfigs").Get<KafkaConsumerServiceConfigs>();
            if(kafkaConsumerConfigs != null)
                foreach (var config in kafkaConsumerConfigs.Configs)
                {
                    var backingService = backingServices.First(x => x.Name == config.BackingServiceName);
                    serviceCollection.AddSingleton<IKafkaConsumerService>(x => new KafkaConsumerService(config, backingService));
                }
            
            //TODO hate wrapping this, figure out arrays
            var azureConsumerConfigs = builder.Configuration.GetSection("AzureServiceBusConsumerServiceConfigs").Get<AzureServiceBusConsumerServiceConfigs>();
            if(azureConsumerConfigs != null)
                foreach (var config in azureConsumerConfigs.Configs)
                {
                    var backingService = backingServices.First(x => x.Name == config.BackingServiceName);
                    serviceCollection.AddSingleton<IAzureServiceBusConsumerService>(x => new AzureServiceBusConsumerService(config, backingService));
                }
            
            serviceCollection.AddHostedService<MainService>();
        })
        .UseSerilog();