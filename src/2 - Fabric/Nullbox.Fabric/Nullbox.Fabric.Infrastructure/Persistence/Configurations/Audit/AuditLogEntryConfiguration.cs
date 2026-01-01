using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Audit;
using Nullbox.Fabric.Domain.Entities.Audit;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Audit;

public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToContainer("Audit");

        builder.HasPartitionKey(x => x.PartitionKey);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0);

        builder.Property(x => x.PartitionKey)
            .IsRequired();

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.PayloadHash)
            .IsRequired();

        builder.Property(x => x.Kind)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.DurationMs)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Error);

        builder.Property(x => x.TraceId)
            .IsRequired();

        builder.Property(x => x.UserId);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.ToTable(tb => tb.HasCheckConstraint("audit_log_entry_kind_check", $"\"Kind\" IN ({string.Join(",", Enum.GetValues<AuditKind>().Select(e => $"'{e}'"))})"));

        builder.ToTable(tb => tb.HasCheckConstraint("audit_log_entry_status_check", $"\"Status\" IN ({string.Join(",", Enum.GetValues<AuditStatus>().Select(e => $"'{e}'"))})"));

        builder.Ignore(e => e.DomainEvents);
    }
}