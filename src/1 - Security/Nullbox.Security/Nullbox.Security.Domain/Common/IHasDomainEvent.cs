using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.DomainEvents.HasDomainEventInterface", Version = "1.0")]

namespace Nullbox.Security.Domain.Common;

public interface IHasDomainEvent
{
    List<DomainEvent> DomainEvents { get; set; }
}