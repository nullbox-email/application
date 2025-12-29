using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Nullbox.Auth.EntraExternalId.Application.Common.Interfaces;
using Nullbox.Auth.EntraExternalId.Domain.Common;
using Nullbox.Auth.EntraExternalId.Domain.Common.Interfaces;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    private readonly IDomainEventService _domainEventService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
    {
        _domainEventService = domainEventService;
    }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureModel(modelBuilder);
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