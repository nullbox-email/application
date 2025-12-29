using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Aliases;
using Nullbox.Fabric.Domain.Deliveries;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Aliases;

public class AliasRuleConfiguration : IEntityTypeConfiguration<AliasRule>
{
    public void Configure(EntityTypeBuilder<AliasRule> builder)
    {
        builder.ToContainer("AliasSenderPolicies");

        builder.HasPartitionKey(x => x.AliasId);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0);

        builder.Property(x => x.AliasId)
            .IsRequired();

        builder.Property(x => x.RuleKind)
            .IsRequired();

        builder.Property(x => x.Domain)
            .IsRequired();

        builder.Property(x => x.Host)
            .IsRequired();

        builder.Property(x => x.Email)
            .IsRequired();

        builder.Property(x => x.Decision)
            .IsRequired();

        builder.Property(x => x.IsEnabled)
            .IsRequired();

        builder.Property(x => x.Source)
            .IsRequired();

        builder.ToTable(tb => tb.HasCheckConstraint("alias_rule_rule_kind_check", $"\"RuleKind\" IN ({string.Join(",", Enum.GetValues<AliasRuleKind>().Select(e => $"'{e}'"))})"));

        builder.ToTable(tb => tb.HasCheckConstraint("alias_rule_decision_check", $"\"Decision\" IN ({string.Join(",", Enum.GetValues<DeliveryDecision>().Select(e => $"'{e}'"))})"));

        builder.ToTable(tb => tb.HasCheckConstraint("alias_rule_source_check", $"\"Source\" IN ({string.Join(",", Enum.GetValues<AliasRuleSource>().Select(e => $"'{e}'"))})"));

        builder.Ignore(e => e.DomainEvents);
    }
}