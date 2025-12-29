using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Api.Services;

[assembly: IntentTemplate("Aryzac.HostedServicesConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Api.Configuration;

public static class HostedServicesConfiguration
{
    public static IServiceCollection ConfigureHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<RegisterProductsHostedService>();
        return services;
    }
}