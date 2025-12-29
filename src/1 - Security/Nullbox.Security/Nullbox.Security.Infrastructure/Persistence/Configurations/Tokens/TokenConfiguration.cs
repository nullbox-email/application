using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Security.Domain.Entities.Tokens;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Security.Infrastructure.Persistence.Configurations.Tokens;

public class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToContainer("Tokens");

        builder.HasPartitionKey(x => x.Id);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0);

        builder.Property(x => x.SerializedToken)
            .IsRequired();

        builder.Property(x => x.Subject)
            .IsRequired();

        builder.Property(x => x.Audience)
            .IsRequired();

        builder.Property(x => x.Issuer)
            .IsRequired();

        builder.Property(x => x.IssuedAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.NotBefore)
            .IsRequired();

        builder.Property(x => x.IsRevoked)
            .IsRequired();

        builder.OwnsMany(x => x.Claims, ConfigureClaims);

        builder.Ignore(e => e.DomainEvents);
    }

    public static void ConfigureClaims(OwnedNavigationBuilder<Token, Claim> builder)
    {
        builder.WithOwner();

        builder.Property(x => x.TokenId)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();
    }
}