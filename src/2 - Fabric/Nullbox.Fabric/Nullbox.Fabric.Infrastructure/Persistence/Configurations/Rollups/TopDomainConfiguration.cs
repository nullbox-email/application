using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Rollups;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Rollups;

public class TopDomainConfiguration : IEntityTypeConfiguration<TopDomain>
{
    public void Configure(EntityTypeBuilder<TopDomain> builder)
    {
        builder.ToContainer("TopDomains");

        builder.HasPartitionKey(x => x.WindowKey);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .ValueGeneratedNever();

        builder.Property(x => x.WindowKey)
            .IsRequired();

        builder.Property(x => x.WindowStart)
            .IsRequired();

        builder.Property(x => x.WindowEnd)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.OwnsMany(x => x.Items, ConfigureItems);

        builder.Ignore(e => e.DomainEvents);
    }

    public static void ConfigureItems(OwnedNavigationBuilder<TopDomain, TopDomainItem> builder)
    {
        builder.WithOwner()
                .HasForeignKey(x => x.TopTargetDomainId);

        builder.Property(x => x.TopTargetDomainId)
            .IsRequired();

        builder.Property(x => x.Domain)
            .IsRequired();

        builder.Property(x => x.Total)
            .IsRequired();

        builder.Property(x => x.Forwarded)
            .IsRequired();

        builder.Property(x => x.Dropped)
            .IsRequired();

        builder.Property(x => x.Quarantined)
            .IsRequired();

        builder.Property(x => x.Delivered)
            .IsRequired();

        builder.Property(x => x.Failed)
            .IsRequired();

        builder.Property(x => x.Bandwidth)
            .IsRequired();
    }
}