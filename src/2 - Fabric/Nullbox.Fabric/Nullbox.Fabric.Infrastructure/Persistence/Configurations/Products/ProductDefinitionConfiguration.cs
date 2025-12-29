using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Products;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Products;

public class ProductDefinitionConfiguration : IEntityTypeConfiguration<ProductDefinition>
{
    public void Configure(EntityTypeBuilder<ProductDefinition> builder)
    {
        builder.ToContainer("Products");

        builder.HasPartitionKey(x => x.Id);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.IsActive)
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