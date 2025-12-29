using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Deliveries;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Aliases;

public class AliasSenderConfiguration : IEntityTypeConfiguration<AliasSender>
{
    public void Configure(EntityTypeBuilder<AliasSender> builder)
    {
        builder.ToContainer("AliasSenderPolicies");

        builder.HasPartitionKey(x => x.AliasId);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0);

        builder.Property(x => x.AliasId)
            .IsRequired();

        builder.Property(x => x.Email)
            .IsRequired();

        builder.Property(x => x.Domain)
            .IsRequired();

        builder.Property(x => x.FirstSeenAt)
            .IsRequired();

        builder.Property(x => x.LastSeenAt)
            .IsRequired();

        builder.Property(x => x.SeenCount)
            .IsRequired();

        builder.Property(x => x.LastDecision);

        builder.ToTable(tb => tb.HasCheckConstraint("alias_sender_last_decision_check", $"\"LastDecision\" IS NULL OR \"LastDecision\" IN ({string.Join(",", Enum.GetValues<DeliveryDecision>().Select(e => $"'{e}'"))})"));

        builder.Ignore(e => e.DomainEvents);
    }
}