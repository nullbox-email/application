using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;

public class DeliveryActionDecisionedV1QuarantineReasonDto
{
    public DeliveryActionDecisionedV1QuarantineReasonDto()
    {
    }

    public QuarantineReason Reason { get; set; }
    public string? Message { get; set; }

    public static DeliveryActionDecisionedV1QuarantineReasonDto Create(QuarantineReason reason, string? message)
    {
        return new DeliveryActionDecisionedV1QuarantineReasonDto
        {
            Reason = reason,
            Message = message
        };
    }
}