using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Aliases;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class AliasLearningModeSchedule : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected AliasLearningModeSchedule()
    {
        Window = null!;
    }
    public Guid Id { get; private set; }

    public string Window { get; private set; }

    public Guid AliasId { get; private set; }

    public Guid MailboxId { get; private set; }

    public DateTimeOffset DueDate { get; private set; }

    public DateTimeOffset? ProcessedAt { get; private set; }

    public ScheduleStatus Status { get; private set; } = ScheduleStatus.Pending;

    public List<DomainEvent> DomainEvents { get; set; } = [];
}