using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Entities.Deliveries;

[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Events.Deliveries;

public class DeliveryActionDecisionedDomainEvent : DomainEvent
{
    public DeliveryActionDecisionedDomainEvent(DeliveryAction deliveryAction)
    {
        DeliveryAction = deliveryAction;
    }

    public DeliveryAction DeliveryAction { get; }
}