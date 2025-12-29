using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Endura.HostedServicesConfiguration", Version = "1.0")]

namespace Nullbox.Security.Api.Configuration;

public static class HostedServicesConfiguration
{
    public static IServiceCollection ConfigureHostedServices(this IServiceCollection services)
    {
        return services;
    }
}