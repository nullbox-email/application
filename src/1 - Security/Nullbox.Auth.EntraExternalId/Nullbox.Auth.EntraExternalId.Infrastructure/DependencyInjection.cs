using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nullbox.Auth.EntraExternalId.Application.Common.Interfaces;
using Nullbox.Auth.EntraExternalId.Domain.Common.Interfaces;
using Nullbox.Auth.EntraExternalId.Infrastructure.Caching;
using Nullbox.Auth.EntraExternalId.Infrastructure.Configuration;
using Nullbox.Auth.EntraExternalId.Infrastructure.Persistence;
using Nullbox.Auth.EntraExternalId.Infrastructure.Services;

[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseInMemoryDatabase("DefaultConnection");
            options.UseLazyLoadingProxies();
        });
        services.AddDistributedMemoryCache();
        services.AddSingleton<IDistributedCacheWithUnitOfWork, DistributedCacheWithUnitOfWork>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddMassTransitConfiguration(configuration);
        services.AddHttpClients(configuration);
        return services;
    }
}