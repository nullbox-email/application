using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Statistics;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Statistics;

public class TrafficStatisticConfiguration : IEntityTypeConfiguration<TrafficStatistic>
{
    public void Configure(EntityTypeBuilder<TrafficStatistic> builder)
    {
        builder.ToContainer("TrafficStatistics");

        builder.HasPartitionKey(x => x.PartitionKey);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
                .ValueGeneratedNever();

        builder.Property(x => x.PartitionKey)
            .IsRequired();

        builder.Property(x => x.BucketKey)
            .IsRequired();

        builder.Property(x => x.BucketStartAt)
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

        builder.Property(x => x.Aliases)
                .IsRequired();

        builder.Property(x => x.Mailboxes)
                .IsRequired();

        builder.Property(x => x.LastCreatedEventId);

        builder.Property(x => x.LastDecisionedEventId);

        builder.Property(x => x.LastCompletedEventId);

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}