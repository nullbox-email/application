using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;

public record DeliveryActionCompleteActivitiesV1
{
    public DeliveryActionCompleteActivitiesV1()
    {
        PartitionKey = null!;
        Source = null!;
        SenderDisplay = null!;
        SenderDomain = null!;
        RecipientDisplay = null!;
        RecipientDomain = null!;
        Subject = null!;
        SubjectHash = null!;
    }

    public Guid Id { get; init; }
    public string PartitionKey { get; init; }
    public Guid? AliasId { get; init; }
    public Guid? MailboxId { get; init; }
    public Guid? AccountId { get; init; }
    public string Source { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset ReceivedAt { get; init; }
    public string? Alias { get; init; }
    public string? RoutingKey { get; init; }
    public string? Domain { get; init; }
    public string SenderDisplay { get; init; }
    public string SenderDomain { get; init; }
    public string RecipientDisplay { get; init; }
    public string RecipientDomain { get; init; }
    public string? MessageId { get; init; }
    public string Subject { get; init; }
    public string SubjectHash { get; init; }
    public bool HasAttachments { get; init; }
    public int AttachmentsCount { get; init; }
    public long Size { get; init; }
    public DeliveryDecision DeliveryDecision { get; init; }
    public DeliveryActionCompletedV1DropReasonDto? DropReason { get; init; }
    public DeliveryActionCompletedV1QuarantineReasonDto? QuarantineReason { get; init; }
    public string? ForwardTo { get; init; }
    public string? ForwardFrom { get; init; }
    public string? Provider { get; init; }
    public ProviderStatus? ProviderStatus { get; init; }
    public string? ProviderMessageId { get; init; }
    public string? ProviderError { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public string? DedupKey { get; init; }
}