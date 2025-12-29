using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.DomainEvents.DomainEventBase", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Common;

public abstract class DomainEvent
{
    protected DomainEvent()
    {
        DateOccurred = DateTimeOffset.UtcNow;
    }

    public bool IsPublished { get; set; }
    public DateTimeOffset DateOccurred { get; protected set; }
}