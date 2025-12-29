using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;

public class DeliveryActionCompletedV1QuarantineReasonDto
{
    public DeliveryActionCompletedV1QuarantineReasonDto()
    {
    }

    public QuarantineReason Reason { get; set; }
    public string? Message { get; set; }

    public static DeliveryActionCompletedV1QuarantineReasonDto Create(QuarantineReason reason, string? message)
    {
        return new DeliveryActionCompletedV1QuarantineReasonDto
        {
            Reason = reason,
            Message = message
        };
    }
}