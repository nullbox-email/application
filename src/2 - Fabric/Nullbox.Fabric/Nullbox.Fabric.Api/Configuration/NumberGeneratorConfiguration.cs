using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Services.NumberGenerator;

[assembly: IntentTemplate("Endura.HostedServicesConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Api.Configuration;

public static class NumberGeneratorConfiguration
{
    public static IServiceCollection ConfigureNumberGenerator(this IServiceCollection services)
    {
        services.AddOptions<UniqueIdentifierGeneratorSettings>("UniqueIdentifierGenerator");
        services.AddSingleton<IUniqueIdentifierGenerator, UniqueIdentifierGenerator>();

        return services;
    }
}