using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;

public class DeliveryActionDecisionedV1DropReasonDto
{
    public DeliveryActionDecisionedV1DropReasonDto()
    {
    }

    public DropReason Reason { get; set; }
    public string? Message { get; set; }

    public static DeliveryActionDecisionedV1DropReasonDto Create(DropReason reason, string? message)
    {
        return new DeliveryActionDecisionedV1DropReasonDto
        {
            Reason = reason,
            Message = message
        };
    }
}