using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Accounts;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Accounts;

public class EnablementGrantConfiguration : IEntityTypeConfiguration<EnablementGrant>
{
    public void Configure(EntityTypeBuilder<EnablementGrant> builder)
    {
        builder.ToContainer("Enablements");

        builder.HasPartitionKey(x => x.AccountId);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .ValueGeneratedNever();

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.Property(x => x.Kind)
            .IsRequired();

        builder.Property(x => x.ProductKey);

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.StartsAt);

        builder.Property(x => x.EndsAt);

        builder.Property(x => x.DeltaMaxMailboxes);

        builder.Property(x => x.DeltaMaxAliases);

        builder.Property(x => x.DeltaMaxAliasesPerMailbox);

        builder.Property(x => x.DeltaMaxBandwidthBytesPerPeriod);

        builder.Property(x => x.Flags)
            .IsRequired();

        builder.Property(x => x.Source)
            .IsRequired();

        builder.Property(x => x.Reason);

        builder.Property(x => x.IsRevoked)
            .IsRequired();

        builder.ToTable(tb => tb.HasCheckConstraint("enablement_grant_kind_check", $"\"Kind\" IN ({string.Join(",", Enum.GetValues<EnablementKind>().Select(e => $"'{e}'"))})"));

        builder.ToTable(tb => tb.HasCheckConstraint("enablement_grant_source_check", $"\"Source\" IN ({string.Join(",", Enum.GetValues<EnablementSource>().Select(e => $"'{e}'"))})"));

        builder.Ignore(e => e.DomainEvents);
    }
}