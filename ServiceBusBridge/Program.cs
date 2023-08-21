using Destructurama;
using KafkaBridge;
using KafkaBridge.Configuration;
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
            //TODO hate wrapping this, figure out how to use arrays with IOptions
            serviceCollection.Configure<KafkaConsumerServiceConfigs>(builder.Configuration.GetSection("KafkaConsumerConfigs"));
            serviceCollection.Configure<AzureServiceBusConsumerServiceConfigs>(builder.Configuration.GetSection("AzureServiceBusConsumerConfigs"));
            
            serviceCollection.AddBackingServices();
            serviceCollection.AddHostedService<MainService>();
        })
        .UseSerilog();