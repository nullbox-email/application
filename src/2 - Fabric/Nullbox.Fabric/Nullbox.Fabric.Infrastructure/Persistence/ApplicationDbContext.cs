using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Common.Interfaces;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Entities.Activities;
using Nullbox.Fabric.Domain.Entities.Aliases;
using Nullbox.Fabric.Domain.Entities.Audit;
using Nullbox.Fabric.Domain.Entities.Deliveries;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Entities.Markers;
using Nullbox.Fabric.Domain.Entities.Products;
using Nullbox.Fabric.Domain.Entities.Rollups;
using Nullbox.Fabric.Domain.Entities.Statistics;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Accounts;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Activities;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Aliases;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Audit;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Deliveries;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Mailboxes;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Markers;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Products;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Rollups;
using Nullbox.Fabric.Infrastructure.Persistence.Configurations.Statistics;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    private readonly IDomainEventService _domainEventService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
    {
        _domainEventService = domainEventService;
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountRole> AccountRoles { get; set; }
    public DbSet<AccountUserMap> AccountUserMaps { get; set; }
    public DbSet<EffectiveEnablement> EffectiveEnablements { get; set; }
    public DbSet<EnablementGrant> EnablementGrants { get; set; }
    public DbSet<RecentDeliveryAction> RecentDeliveryActions { get; set; }

    public DbSet<Alias> Aliases { get; set; }
    public DbSet<AliasLearningModeSchedule> AliasLearningModeSchedules { get; set; }
    public DbSet<AliasMap> AliasMaps { get; set; }
    public DbSet<AliasRule> AliasRules { get; set; }
    public DbSet<AliasSender> AliasSenders { get; set; }
    public DbSet<AliasSenderDecision> AliasSenderDecisions { get; set; }
    public DbSet<AuditLogEntry> AuditLogEntries { get; set; }
    public DbSet<DeliveryAction> DeliveryActions { get; set; }

    public DbSet<Mailbox> Mailboxes { get; set; }
    public DbSet<MailboxMap> MailboxMaps { get; set; }
    public DbSet<MailboxRoutingKeyMap> MailboxRoutingKeyMaps { get; set; }
    public DbSet<AppliedMarker> AppliedMarkers { get; set; }
    public DbSet<ProductDefinition> ProductDefinitions { get; set; }
    public DbSet<TopAlias> TopAliases { get; set; }
    public DbSet<TopDomain> TopDomains { get; set; }
    public DbSet<TrafficStatistic> TrafficStatistics { get; set; }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        await DispatchEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        DispatchEventsAsync().GetAwaiter().GetResult();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Calling EnsureCreatedAsync is necessary to create the required containers and insert the seed data if present in the model.
    /// However EnsureCreatedAsync should only be called during deployment, not normal operation, as it may cause performance issues.
    /// </summary>
    public async Task EnsureDbCreatedAsync()
    {
        await Database.EnsureCreatedAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureModel(modelBuilder);
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new AccountRoleConfiguration());
        modelBuilder.ApplyConfiguration(new AccountUserMapConfiguration());
        modelBuilder.ApplyConfiguration(new EffectiveEnablementConfiguration());
        modelBuilder.ApplyConfiguration(new EnablementGrantConfiguration());
        modelBuilder.ApplyConfiguration(new RecentDeliveryActionConfiguration());
        modelBuilder.ApplyConfiguration(new AliasConfiguration());
        modelBuilder.ApplyConfiguration(new AliasLearningModeScheduleConfiguration());
        modelBuilder.ApplyConfiguration(new AliasMapConfiguration());
        modelBuilder.ApplyConfiguration(new AliasRuleConfiguration());
        modelBuilder.ApplyConfiguration(new AliasSenderConfiguration());
        modelBuilder.ApplyConfiguration(new AliasSenderDecisionConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogEntryConfiguration());
        modelBuilder.ApplyConfiguration(new DeliveryActionConfiguration());
        modelBuilder.ApplyConfiguration(new MailboxConfiguration());
        modelBuilder.ApplyConfiguration(new MailboxMapConfiguration());
        modelBuilder.ApplyConfiguration(new MailboxRoutingKeyMapConfiguration());
        modelBuilder.ApplyConfiguration(new AppliedMarkerConfiguration());
        modelBuilder.ApplyConfiguration(new ProductDefinitionConfiguration());
        modelBuilder.ApplyConfiguration(new TopAliasConfiguration());
        modelBuilder.ApplyConfiguration(new TopDomainConfiguration());
        modelBuilder.ApplyConfiguration(new TrafficStatisticConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties(typeof(Enum)).HaveConversion<string>();
    }

    private void ConfigureModel(ModelBuilder modelBuilder)
    {
        // Seed data
        // https://rehansaeed.com/migrating-to-entity-framework-core-seed-data/
        /* E.g.
        modelBuilder.Entity<Car>().HasData(
            new Car() { CarId = 1, Make = "Ferrari", Model = "F40" },
            new Car() { CarId = 2, Make = "Ferrari", Model = "F50" },
            new Car() { CarId = 3, Make = "Lamborghini", Model = "Countach" });
        */
    }

    private async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            var domainEventEntity = ChangeTracker
                .Entries<IHasDomainEvent>()
                .SelectMany(x => x.Entity.DomainEvents)
                .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

            if (domainEventEntity is null)
            {
                break;
            }

            domainEventEntity.IsPublished = true;
            await _domainEventService.Publish(domainEventEntity, cancellationToken);
        }
    }
}