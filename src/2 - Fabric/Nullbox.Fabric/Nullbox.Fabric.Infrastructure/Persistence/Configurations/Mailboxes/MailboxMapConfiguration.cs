using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Mailboxes;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Mailboxes;

public class MailboxMapConfiguration : IEntityTypeConfiguration<MailboxMap>
{
    public void Configure(EntityTypeBuilder<MailboxMap> builder)
    {
        builder.ToContainer("MailboxMaps");

        builder.HasPartitionKey(x => x.Id);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0);

        builder.Property(x => x.MailboxId)
            .IsRequired();

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.Property(x => x.AutoCreateAlias)
                .IsRequired();

        builder.Property(x => x.EmailAddress)
                .IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}