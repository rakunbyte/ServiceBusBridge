using BackingServices;
using BackingServices.Common;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaBridge.Configuration;

public static class BackingServiceSetup
{
    public static void AddBackingServices(this IServiceCollection services)
    {
        services.AddScoped<IBackingService, AccountCreatedService>();
        services.AddScoped<IBackingService, DeliveryScheduledService>();
    }
}