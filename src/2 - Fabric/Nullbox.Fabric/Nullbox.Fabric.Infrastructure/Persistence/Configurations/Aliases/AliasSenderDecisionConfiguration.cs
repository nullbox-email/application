using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Aliases;

public class AliasSenderDecisionConfiguration : IEntityTypeConfiguration<AliasSenderDecision>
{
    public void Configure(EntityTypeBuilder<AliasSenderDecision> builder)
    {
        builder.ToContainer("AliasSenderPolicies");

        builder.HasPartitionKey(x => x.AliasId);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0);

        builder.Property(x => x.AliasId)
            .IsRequired();

        builder.Property(x => x.LearningUntil)
            .IsRequired();

        builder.Ignore(e => e.DomainEvents);
    }
}