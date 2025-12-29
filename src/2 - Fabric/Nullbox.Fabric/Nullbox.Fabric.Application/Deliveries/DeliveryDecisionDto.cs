using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Deliveries;

public class DeliveryDecisionDto
{
    public DeliveryDecisionDto()
    {
        PartitionKey = null!;
    }

    public Guid DeliveryActionId { get; set; }
    public string PartitionKey { get; set; }

    public EmailProcessingAction Action { get; set; }
    public string? ForwardTo { get; set; }
    public string? ForwardFrom { get; set; }
    public string? Reason { get; set; }

    public static DeliveryDecisionDto Create(
            Guid deliveryActionId,
            string partitionKey,
            EmailProcessingAction action,
            string? forwardTo,
            string? forwardFrom,
            string? reason)
    {
        return new DeliveryDecisionDto
        {
            DeliveryActionId = deliveryActionId,
            PartitionKey = partitionKey,
            Action = action,
            ForwardTo = forwardTo,
            ForwardFrom = forwardFrom,
            Reason = reason
        };
    }
}