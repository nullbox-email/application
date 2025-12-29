using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Security.Domain.Entities.Users;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Security.Infrastructure.Persistence.Configurations.Users;

public class ExternalUserConfiguration : IEntityTypeConfiguration<ExternalUser>
{
    public void Configure(EntityTypeBuilder<ExternalUser> builder)
    {
        builder.ToContainer("ExternalUsers");

        builder.HasPartitionKey(x => x.Context);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0);

        builder.Property(x => x.Context)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}