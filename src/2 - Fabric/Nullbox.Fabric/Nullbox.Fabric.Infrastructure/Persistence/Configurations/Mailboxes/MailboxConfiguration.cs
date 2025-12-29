using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Mailboxes;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Mailboxes;

public class MailboxConfiguration : IEntityTypeConfiguration<Mailbox>
{
    public void Configure(EntityTypeBuilder<Mailbox> builder)
    {
        builder.ToContainer("Mailboxes");

        builder.HasPartitionKey(x => x.AccountId);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .ValueGeneratedNever();

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.Property(x => x.RoutingKey)
            .IsRequired();

        builder.Property(x => x.Name)
                .IsRequired();

        builder.Property(x => x.Domain)
                .IsRequired();

        builder.Property(x => x.AutoCreateAlias)
                .IsRequired();

        builder.Property(x => x.EmailAddress)
                .IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}