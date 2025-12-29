using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Events.Accounts;

public class EnablementGrantCreatedDomainEvent : DomainEvent
{
    public EnablementGrantCreatedDomainEvent(EnablementGrant enablementGrant)
    {
        EnablementGrant = enablementGrant;
    }

    public EnablementGrant EnablementGrant { get; }
}