using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nullbox.Fabric.Domain.Aliases;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Persistence.Configurations.Aliases;

public class AliasLearningModeScheduleConfiguration : IEntityTypeConfiguration<AliasLearningModeSchedule>
{
    public void Configure(EntityTypeBuilder<AliasLearningModeSchedule> builder)
    {
        builder.ToContainer("Schedules");

        builder.HasPartitionKey(x => x.Window);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnOrder(0);

        builder.Property(x => x.Window)
            .IsRequired();

        builder.Property(x => x.AliasId)
                .IsRequired();

        builder.Property(x => x.MailboxId)
                .IsRequired();

        builder.Property(x => x.DueDate)
            .IsRequired();

        builder.Property(x => x.ProcessedAt);

        builder.Property(x => x.Status)
                .IsRequired();

        builder.ToTable(tb => tb.HasCheckConstraint("alias_learning_mode_schedule_status_check", $"\"Status\" IN ({string.Join(",", Enum.GetValues<ScheduleStatus>().Select(e => $"'{e}'"))})"));

        builder.Ignore(e => e.DomainEvents);
    }
}