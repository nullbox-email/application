using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;

public class DeliveryActionCreatedV1DropReasonDto
{
    public DeliveryActionCreatedV1DropReasonDto()
    {
    }

    public DropReason Reason { get; set; }
    public string? Message { get; set; }

    public static DeliveryActionCreatedV1DropReasonDto Create(DropReason reason, string? message)
    {
        return new DeliveryActionCreatedV1DropReasonDto
        {
            Reason = reason,
            Message = message
        };
    }
}