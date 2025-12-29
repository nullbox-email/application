using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;
using Nullbox.Fabric.Domain.ValueObjects;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Activities;

public partial class RecentDeliveryAction
{
    public RecentDeliveryAction(string id,
        Guid partitionKey,
        Guid deliveryActionId,
        DateTimeOffset receivedAt,
        string sortKey,
        string senderDisplay,
        string senderDomain,
        string recipientDisplay,
        string recipientDomain,
        string subject,
        string subjectHash,
        bool hasAttachments,
        int attachmentsCount,
        long size,
        DeliveryDecision deliveryDecision,
        DropReasonValueObject? dropReason,
        QuarantineReasonValueObject? quarantineReason,
        string? forwardTo,
        string? forwardFrom,
        string? provider,
        ProviderStatus? providerStatus,
        string? providerMessageId,
        string? providerError,
        DateTimeOffset? completedAt,
        DateTimeOffset? updatedAt)
    {
        Id = id;
        PartitionKey = partitionKey;
        DeliveryActionId = deliveryActionId;
        ReceivedAt = receivedAt;
        SortKey = sortKey;
        SenderDisplay = senderDisplay;
        SenderDomain = senderDomain;
        RecipientDisplay = recipientDisplay;
        RecipientDomain = recipientDomain;
        Subject = subject;
        SubjectHash = subjectHash;
        HasAttachments = hasAttachments;
        AttachmentsCount = attachmentsCount;
        Size = size;
        DeliveryDecision = deliveryDecision;
        DropReason = dropReason;
        QuarantineReason = quarantineReason;
        ForwardTo = forwardTo;
        ForwardFrom = forwardFrom;
        Provider = provider;
        ProviderStatus = providerStatus;
        ProviderMessageId = providerMessageId;
        ProviderError = providerError;
        CompletedAt = completedAt;
        UpdatedAt = updatedAt;
    }

    public void Update(
        string senderDisplay,
        string senderDomain,
        string recipientDisplay,
        string recipientDomain,
        string subject,
        string subjectHash,
        bool hasAttachments,
        int attachmentsCount,
        long size,
        DeliveryDecision deliveryDecision,
        DropReasonValueObject? dropReason,
        QuarantineReasonValueObject? quarantineReason,
        string? forwardTo,
        string? forwardFrom,
        string? provider,
        ProviderStatus? providerStatus,
        string? providerMessageId,
        string? providerError,
        DateTimeOffset? completedAt,
        DateTimeOffset? updatedAt)
    {
        SenderDisplay = senderDisplay;
        SenderDomain = senderDomain;
        RecipientDisplay = recipientDisplay;
        RecipientDomain = recipientDomain;
        Subject = subject;
        SubjectHash = subjectHash;
        HasAttachments = hasAttachments;
        AttachmentsCount = attachmentsCount;
        Size = size;
        DeliveryDecision = deliveryDecision;
        DropReason = dropReason;
        QuarantineReason = quarantineReason;
        ForwardTo = forwardTo;
        ForwardFrom = forwardFrom;
        Provider = provider;
        ProviderStatus = providerStatus;
        ProviderMessageId = providerMessageId;
        ProviderError = providerError;
        CompletedAt = completedAt;
        UpdatedAt = updatedAt;
    }
}