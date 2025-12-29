using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Aliases;

public class AliasConfiguration : IEntityTypeConfiguration<Alias>
{
    public void Configure(EntityTypeBuilder<Alias> builder)
    {
        builder.ToContainer("Aliases");

        builder.HasPartitionKey(x => x.MailboxId);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0);

        builder.Property(x => x.MailboxId)
            .IsRequired();

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.LocalPart)
            .IsRequired();

        builder.Property(x => x.IsEnabled)
                .IsRequired();

        builder.Property(x => x.DirectPassthrough)
                .IsRequired();

        builder.Property(x => x.LearningMode)
                .IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}