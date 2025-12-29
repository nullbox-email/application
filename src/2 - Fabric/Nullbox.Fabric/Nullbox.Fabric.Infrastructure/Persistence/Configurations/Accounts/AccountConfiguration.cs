using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Accounts;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToContainer("Accounts");

        builder.HasPartitionKey(x => x.Id);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
                .ValueGeneratedNever();

        builder.Property(x => x.Name)
                .IsRequired();

        builder.OwnsMany(x => x.Users, ConfigureUsers);

        builder.Ignore(e => e.DomainEvents);
    }

    public static void ConfigureUsers(OwnedNavigationBuilder<Account, AccountUser> builder)
    {
        builder.WithOwner();

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.Property(x => x.UserProfileId)
            .IsRequired();

        builder.Property(x => x.EmailAddress)
            .IsRequired();

        builder.Property(x => x.RoleId)
            .IsRequired();
    }
}