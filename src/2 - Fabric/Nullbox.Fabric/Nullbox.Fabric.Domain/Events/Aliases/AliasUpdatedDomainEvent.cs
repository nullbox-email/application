using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Events.Aliases;

public class AliasUpdatedDomainEvent : DomainEvent
{
    public AliasUpdatedDomainEvent(Alias alias)
    {
        Alias = alias;
    }

    public Alias Alias { get; }
}