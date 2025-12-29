using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Statistics;

public class DeliveryActionDecisionedV1Handler : IIntegrationEventHandler<DeliveryActionDecisionedV1Event>
{
    private readonly IMessageBus _messageBus;
    public DeliveryActionDecisionedV1Handler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(
            DeliveryActionDecisionedV1Event message,
            CancellationToken cancellationToken = default)
    {
        _messageBus.Send(new DeliveryActionDecisionActivitiesV1
        {
            Id = message.Id,
            PartitionKey = message.PartitionKey,
            AliasId = message.AliasId,
            MailboxId = message.MailboxId,
            AccountId = message.AccountId,
            Source = message.Source,
            CreatedAt = message.CreatedAt,
            ReceivedAt = message.ReceivedAt,
            Alias = message.Alias,
            RoutingKey = message.RoutingKey,
            Domain = message.Domain,
            SenderDisplay = message.SenderDisplay,
            SenderDomain = message.SenderDomain,
            RecipientDisplay = message.RecipientDisplay,
            RecipientDomain = message.RecipientDomain,
            MessageId = message.MessageId,
            Subject = message.Subject,
            SubjectHash = message.SubjectHash,
            HasAttachments = message.HasAttachments,
            AttachmentsCount = message.AttachmentsCount,
            Size = message.Size,
            DeliveryDecision = message.DeliveryDecision,
            DropReason = message.DropReason is not null
                    ? new DeliveryActionCreatedV1DropReasonDto
                    {
                        Reason = message.DropReason.Reason,
                        Message = message.DropReason?.Message
                    }
                    : null,
            QuarantineReason = message.QuarantineReason is not null
                    ? new DeliveryActionCreatedV1QuarantineReasonDto
                    {
                        Reason = message.QuarantineReason.Reason,
                        Message = message.QuarantineReason?.Message
                    }
                    : null,
            ForwardTo = message.ForwardTo,
            ForwardFrom = message.ForwardFrom,
            Provider = message.Provider,
            ProviderStatus = message.ProviderStatus,
            ProviderMessageId = message.ProviderMessageId,
            ProviderError = message.ProviderError,
            CompletedAt = message.CompletedAt,
            DedupKey = message.DedupKey
        });
        _messageBus.Send(new DeliveryActionDecisionRollupsV1
        {
            Id = message.Id,
            PartitionKey = message.PartitionKey,
            AliasId = message.AliasId,
            MailboxId = message.MailboxId,
            AccountId = message.AccountId,
            Source = message.Source,
            CreatedAt = message.CreatedAt,
            ReceivedAt = message.ReceivedAt,
            Alias = message.Alias,
            RoutingKey = message.RoutingKey,
            Domain = message.Domain,
            SenderDisplay = message.SenderDisplay,
            SenderDomain = message.SenderDomain,
            RecipientDisplay = message.RecipientDisplay,
            RecipientDomain = message.RecipientDomain,
            MessageId = message.MessageId,
            Subject = message.Subject,
            SubjectHash = message.SubjectHash,
            HasAttachments = message.HasAttachments,
            AttachmentsCount = message.AttachmentsCount,
            Size = message.Size,
            DeliveryDecision = message.DeliveryDecision,
            DropReason = message.DropReason is not null
                    ? new DeliveryActionDecisionedV1DropReasonDto
                    {
                        Reason = message.DropReason.Reason,
                        Message = message.DropReason?.Message
                    }
                    : null,
            QuarantineReason = message.QuarantineReason is not null
                    ? new DeliveryActionDecisionedV1QuarantineReasonDto
                    {
                        Reason = message.QuarantineReason.Reason,
                        Message = message.QuarantineReason?.Message
                    }
                    : null,
            ForwardTo = message.ForwardTo,
            ForwardFrom = message.ForwardFrom,
            Provider = message.Provider,
            ProviderStatus = message.ProviderStatus,
            ProviderMessageId = message.ProviderMessageId,
            ProviderError = message.ProviderError,
            CompletedAt = message.CompletedAt,
            DedupKey = message.DedupKey
        });
        _messageBus.Send(new DeliveryActionDecisionStatisticsV1
        {
            Id = message.Id,
            PartitionKey = message.PartitionKey,
            AliasId = message.AliasId,
            MailboxId = message.MailboxId,
            AccountId = message.AccountId,
            Source = message.Source,
            CreatedAt = message.CreatedAt,
            ReceivedAt = message.ReceivedAt,
            Alias = message.Alias,
            RoutingKey = message.RoutingKey,
            Domain = message.Domain,
            SenderDisplay = message.SenderDisplay,
            SenderDomain = message.SenderDomain,
            RecipientDisplay = message.RecipientDisplay,
            RecipientDomain = message.RecipientDomain,
            MessageId = message.MessageId,
            Subject = message.Subject,
            SubjectHash = message.SubjectHash,
            HasAttachments = message.HasAttachments,
            AttachmentsCount = message.AttachmentsCount,
            Size = message.Size,
            DeliveryDecision = message.DeliveryDecision,
            DropReason = message.DropReason is not null
                    ? new DeliveryActionCompletedV1DropReasonDto
                    {
                        Reason = message.DropReason.Reason,
                        Message = message.DropReason?.Message
                    }
                    : null,
            QuarantineReason = message.QuarantineReason is not null
                    ? new DeliveryActionCompletedV1QuarantineReasonDto
                    {
                        Reason = message.QuarantineReason.Reason,
                        Message = message.QuarantineReason?.Message
                    }
                    : null,
            ForwardTo = message.ForwardTo,
            ForwardFrom = message.ForwardFrom,
            Provider = message.Provider,
            ProviderStatus = message.ProviderStatus,
            ProviderMessageId = message.ProviderMessageId,
            ProviderError = message.ProviderError,
            CompletedAt = message.CompletedAt,
            DedupKey = message.DedupKey
        });
    }
}