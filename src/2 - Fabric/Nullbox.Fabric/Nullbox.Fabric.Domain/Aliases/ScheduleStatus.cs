using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Aliases;

public enum ScheduleStatus
{
    Pending,
    Processed,
    Skipped
}