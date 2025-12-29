using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Security.Domain.Entities.Users;
using Nullbox.Security.Domain.Users;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Security.Infrastructure.Persistence.Configurations.Users;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToContainer("Users");

        builder.HasPartitionKey(x => x.Id);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.OwnsOne(x => x.EmailAddress, ConfigureEmailAddress)
                .Navigation(x => x.EmailAddress).IsRequired();

        builder.Property(x => x.Status)
                .IsRequired();

        builder.ToTable(tb => tb.HasCheckConstraint("user_profile_status_check", $"\"Status\" IN ({string.Join(",", Enum.GetValues<UserStatus>().Select(e => $"'{e}'"))})"));

        builder.Ignore(e => e.DomainEvents);
    }

    public static void ConfigureEmailAddress(OwnedNavigationBuilder<UserProfile, EmailAddressValueObject> builder)
    {
        builder.Property(x => x.Value)
            .IsRequired();

        builder.Property(x => x.NormalizedValue)
            .IsRequired();
    }
}