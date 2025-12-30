using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nullbox.Security.Application.Common.Interfaces;
using Nullbox.Security.Domain.Common.Interfaces;
using Nullbox.Security.Domain.Repositories.Tokens;
using Nullbox.Security.Domain.Repositories.Users;
using Nullbox.Security.Infrastructure.Caching;
using Nullbox.Security.Infrastructure.Configuration;
using Nullbox.Security.Infrastructure.Persistence;
using Nullbox.Security.Infrastructure.Repositories.Tokens;
using Nullbox.Security.Infrastructure.Repositories.Users;
using Nullbox.Security.Infrastructure.Services;

[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Nullbox.Security.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            // [IntentIgnore]
            options.UseCosmos(
                configuration["ConnectionStrings:cosmos-db"],
                configuration["Cosmos:DatabaseName"],
                cosmosOptions =>
                {
#if DEBUG
                        // Required for emulator
                        cosmosOptions.HttpClientFactory(() =>
                        {
                            var httpMessageHandler = new HttpClientHandler();
                            httpMessageHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                            return new HttpClient(httpMessageHandler);
                        });
                        cosmosOptions.ConnectionMode(connectionMode: ConnectionMode.Gateway);
                        cosmosOptions.LimitToEndpoint();
#endif
                });
            options.UseLazyLoadingProxies();
        });
        services.AddDistributedMemoryCache();
        services.AddSingleton<IDistributedCacheWithUnitOfWork, DistributedCacheWithUnitOfWork>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddTransient<ITokenRepository, TokenRepository>();
        services.AddTransient<IExternalUserRepository, ExternalUserRepository>();
        services.AddTransient<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddMassTransitConfiguration(configuration);
        services.AddHttpClients(configuration);
        return services;
    }
}