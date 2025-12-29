using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Nullbox.Security.Application.Common.Interfaces;
using Nullbox.Security.Domain.Common;
using Nullbox.Security.Domain.Common.Interfaces;
using Nullbox.Security.Domain.Entities.Tokens;
using Nullbox.Security.Domain.Entities.Users;
using Nullbox.Security.Infrastructure.Persistence.Configurations.Tokens;
using Nullbox.Security.Infrastructure.Persistence.Configurations.Users;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace Nullbox.Security.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    private readonly IDomainEventService _domainEventService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
    {
        _domainEventService = domainEventService;
    }

    public DbSet<Token> Tokens { get; set; }

    public DbSet<ExternalUser> ExternalUsers { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

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
        modelBuilder.ApplyConfiguration(new TokenConfiguration());
        modelBuilder.ApplyConfiguration(new ExternalUserConfiguration());
        modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
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