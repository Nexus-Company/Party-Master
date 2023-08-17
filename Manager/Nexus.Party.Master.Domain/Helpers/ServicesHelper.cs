using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nexus.Party.Master.Domain.Helpers;
public static class ServicesHelper
{
    public static IServiceCollection AddCustomService<TService>(this IServiceCollection services) 
        where TService : BackgroundService
    {
        services = services.AddSingleton<TService>();
        return services.AddHostedService(provider => provider.GetService<TService>()!);
    }

    public static IServiceCollection AddCustomService<TService, TImplementation>(this IServiceCollection services)
          where TImplementation : BackgroundService, TService
          where TService : class
    {
        services = services.AddSingleton<TService, TImplementation>();
        return services.AddHostedService(provider => provider.GetService<TImplementation>()!);
    }
}