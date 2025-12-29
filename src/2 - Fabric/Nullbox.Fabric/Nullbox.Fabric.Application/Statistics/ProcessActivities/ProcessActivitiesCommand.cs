using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Deliveries;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Statistics.ProcessActivities;

public class ProcessActivitiesCommand : IRequest, ICommand
{
    public ProcessActivitiesCommand(Guid id,
        string partitionKey,
        Guid? aliasId,
        Guid? mailboxId,
        Guid? accountId,
        string source,
        DateTimeOffset createdAt,
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
        long size,
        DeliveryDecision deliveryDecision,
        ProcessActivitiesCommandDropReasonDto? dropReason,
        ProcessActivitiesCommandQuarantineReasonDto? quarantineReason,
        string? forwardTo,
        string? forwardFrom,
        string? provider,
        ProviderStatus? providerStatus,
        string? providerMessageId,
        string? providerError,
        DateTimeOffset? completedAt,
        string? dedupKey,
        int wait = 0)
    {
        Id = id;
        PartitionKey = partitionKey;
        AliasId = aliasId;
        MailboxId = mailboxId;
        AccountId = accountId;
        Source = source;
        CreatedAt = createdAt;
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
        DedupKey = dedupKey;
        Wait = wait;
    }

    public Guid Id { get; set; }
    public string PartitionKey { get; set; }
    public Guid? AliasId { get; set; }
    public Guid? MailboxId { get; set; }
    public Guid? AccountId { get; set; }
    public string Source { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public string? Alias { get; set; }
    public string? RoutingKey { get; set; }
    public string? Domain { get; set; }
    public string SenderDisplay { get; set; }
    public string SenderDomain { get; set; }
    public string RecipientDisplay { get; set; }
    public string RecipientDomain { get; set; }
    public string? MessageId { get; set; }
    public string Subject { get; set; }
    public string SubjectHash { get; set; }
    public bool HasAttachments { get; set; }
    public int AttachmentsCount { get; set; }
    public long Size { get; set; }
    public DeliveryDecision DeliveryDecision { get; set; }
    public ProcessActivitiesCommandDropReasonDto? DropReason { get; set; }
    public ProcessActivitiesCommandQuarantineReasonDto? QuarantineReason { get; set; }
    public string? ForwardTo { get; set; }
    public string? ForwardFrom { get; set; }
    public string? Provider { get; set; }
    public ProviderStatus? ProviderStatus { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ProviderError { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? DedupKey { get; set; }
    public int Wait { get; set; }
}