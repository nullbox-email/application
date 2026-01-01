using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Application.Common.Storage;
using Nullbox.Fabric.Domain.Common.Interfaces;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Activities;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Audit;
using Nullbox.Fabric.Domain.Repositories.Deliveries;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Markers;
using Nullbox.Fabric.Domain.Repositories.Products;
using Nullbox.Fabric.Domain.Repositories.Rollups;
using Nullbox.Fabric.Domain.Repositories.Statistics;
using Nullbox.Fabric.Infrastructure.BlobStorage;
using Nullbox.Fabric.Infrastructure.Caching;
using Nullbox.Fabric.Infrastructure.Configuration;
using Nullbox.Fabric.Infrastructure.Partitioning;
using Nullbox.Fabric.Infrastructure.Persistence;
using Nullbox.Fabric.Infrastructure.Repositories.Accounts;
using Nullbox.Fabric.Infrastructure.Repositories.Activities;
using Nullbox.Fabric.Infrastructure.Repositories.Aliases;
using Nullbox.Fabric.Infrastructure.Repositories.Audit;
using Nullbox.Fabric.Infrastructure.Repositories.Deliveries;
using Nullbox.Fabric.Infrastructure.Repositories.Mailboxes;
using Nullbox.Fabric.Infrastructure.Repositories.Markers;
using Nullbox.Fabric.Infrastructure.Repositories.Products;
using Nullbox.Fabric.Infrastructure.Repositories.Rollups;
using Nullbox.Fabric.Infrastructure.Repositories.Statistics;
using Nullbox.Fabric.Infrastructure.Services;

[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure;

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
        services.AddTransient<IBlobStorage, AzureBlobStorage>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<IAccountRoleRepository, AccountRoleRepository>();
        services.AddTransient<IAccountUserMapRepository, AccountUserMapRepository>();
        services.AddTransient<IEffectiveEnablementRepository, EffectiveEnablementRepository>();
        services.AddTransient<IEnablementGrantRepository, EnablementGrantRepository>();
        services.AddTransient<IRecentDeliveryActionRepository, RecentDeliveryActionRepository>();
        services.AddTransient<IAliasRepository, AliasRepository>();
        services.AddTransient<IAliasLearningModeScheduleRepository, AliasLearningModeScheduleRepository>();
        services.AddTransient<IAliasMapRepository, AliasMapRepository>();
        services.AddTransient<IAliasRuleRepository, AliasRuleRepository>();
        services.AddTransient<IAliasSenderRepository, AliasSenderRepository>();
        services.AddTransient<IAliasSenderDecisionRepository, AliasSenderDecisionRepository>();
        services.AddTransient<IAuditLogEntryRepository, AuditLogEntryRepository>();
        services.AddTransient<IDeliveryActionRepository, DeliveryActionRepository>();
        services.AddTransient<IMailboxRepository, MailboxRepository>();
        services.AddTransient<IMailboxMapRepository, MailboxMapRepository>();
        services.AddTransient<IMailboxRoutingKeyMapRepository, MailboxRoutingKeyMapRepository>();
        services.AddTransient<IAppliedMarkerRepository, AppliedMarkerRepository>();
        services.AddTransient<IProductDefinitionRepository, ProductDefinitionRepository>();
        services.AddTransient<ITopAliasRepository, TopAliasRepository>();
        services.AddTransient<ITopDomainRepository, TopDomainRepository>();
        services.AddTransient<ITrafficStatisticRepository, TrafficStatisticRepository>();
        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddMassTransitConfiguration(configuration);
        services.AddHttpClients(configuration);
        
        // [IntentIgnore]
        services.AddScoped<IPartitionKeyScope, PartitionKeyScope>();
        return services;
    }
}