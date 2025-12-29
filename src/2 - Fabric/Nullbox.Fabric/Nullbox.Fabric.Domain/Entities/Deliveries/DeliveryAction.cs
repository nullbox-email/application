using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;
using Nullbox.Fabric.Domain.Events.Deliveries;
using Nullbox.Fabric.Domain.ValueObjects;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Deliveries;

public partial class DeliveryAction
{
    public DeliveryAction(string partitionKey,
        Guid? aliasId,
        Guid? mailboxId,
        Guid? accountId,
        string source,
        DateTimeOffset receivedAt,
        string? alias,
        string? routingKey,
        string? domain,
        string senderDisplay,
        string senderDomain,
        string recipientDisplay,
        string recipientDomain,
        string? messageId,
        string subject,
        string subjectHash,
        bool hasAttachments,
        int attachmentsCount,
        long size)
    {
        PartitionKey = partitionKey;
        AliasId = aliasId;
        MailboxId = mailboxId;
        AccountId = accountId;
        Source = source;
        ReceivedAt = receivedAt;
        Alias = alias;
        RoutingKey = routingKey;
        Domain = domain;
        SenderDisplay = senderDisplay;
        SenderDomain = senderDomain;
        RecipientDisplay = recipientDisplay;
        RecipientDomain = recipientDomain;
        MessageId = messageId;
        Subject = subject;
        SubjectHash = subjectHash;
        HasAttachments = hasAttachments;
        AttachmentsCount = attachmentsCount;
        Size = size;
        DomainEvents.Add(new DeliveryActionCreatedDomainEvent(
            deliveryAction: this));
    }

    public void Drop(DropReason reason, string? message)
    {
        DeliveryDecision = DeliveryDecision.Drop;
        DropReason = new DropReasonValueObject(reason, message);

        DomainEvents.Add(new DeliveryActionDecisionedDomainEvent(
                deliveryAction: this));
    }

    public void Forward(string? forwardTo, string? forwardFrom)
    {
        DeliveryDecision = DeliveryDecision.Forward;
        ForwardTo = forwardTo;
        ForwardFrom = forwardFrom;

        DomainEvents.Add(new DeliveryActionDecisionedDomainEvent(
                deliveryAction: this));
    }

    public void Quarantine(QuarantineReason reason, string? message)
    {
        DeliveryDecision = DeliveryDecision.Quarantine;
        QuarantineReason = new QuarantineReasonValueObject(reason, message);

        DomainEvents.Add(new DeliveryActionDecisionedDomainEvent(
                deliveryAction: this));
    }

    public void Complete(
        string? provider,
        ProviderStatus? providerStatus,
        string? providerMessageId,
        string? providerError)
    {
        Provider = provider;
        ProviderStatus = providerStatus;
        ProviderMessageId = providerMessageId;
        ProviderError = providerError;
        CompletedAt = DateTimeOffset.UtcNow;

        DomainEvents.Add(new DeliveryActionCompletedDomainEvent(
                deliveryAction: this));
    }
}