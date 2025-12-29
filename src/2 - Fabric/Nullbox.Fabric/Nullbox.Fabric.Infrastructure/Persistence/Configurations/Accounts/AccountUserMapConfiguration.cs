using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Accounts;

public class AccountUserMapConfiguration : IEntityTypeConfiguration<AccountUserMap>
{
    public void Configure(EntityTypeBuilder<AccountUserMap> builder)
    {
        builder.ToContainer("AccountMaps");

        builder.HasPartitionKey(x => x.PartitionKey);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
                .ValueGeneratedNever();

        builder.Property(x => x.PartitionKey)
            .IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}