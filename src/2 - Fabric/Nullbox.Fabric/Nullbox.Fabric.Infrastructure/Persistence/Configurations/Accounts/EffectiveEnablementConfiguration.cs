using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Accounts;

public class EffectiveEnablementConfiguration : IEntityTypeConfiguration<EffectiveEnablement>
{
    public void Configure(EntityTypeBuilder<EffectiveEnablement> builder)
    {
        builder.ToContainer("Enablements");

        builder.HasPartitionKey(x => x.AccountId);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .ValueGeneratedNever();

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.Property(x => x.EffectiveProductKey)
                .IsRequired();

        builder.Property(x => x.MaxMailboxes);

        builder.Property(x => x.MaxAliases);

        builder.Property(x => x.MaxAliasesPerMailbox);

        builder.Property(x => x.MaxBandwidthBytesPerPeriod);

        builder.Property(x => x.Flags)
                .IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}