using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Application.Deliveries;
using Nullbox.Fabric.Application.Statistics.ProcessActivities;
using Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Statistics;

public class DeliveryActionDecisionActivitiesV1Handler : IIntegrationEventHandler<DeliveryActionDecisionActivitiesV1>
{
    private readonly ISender _mediator;

    public DeliveryActionDecisionActivitiesV1Handler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(
DeliveryActionDecisionActivitiesV1 message,
CancellationToken cancellationToken = default)
    {
        var command = new ProcessActivitiesCommand(
            id: message.Id,
            partitionKey: message.PartitionKey,
            aliasId: message.AliasId,
            mailboxId: message.MailboxId,
            accountId: message.AccountId,
            source: message.Source,
            createdAt: message.CreatedAt,
            receivedAt: message.ReceivedAt,
            alias: message.Alias,
            routingKey: message.RoutingKey,
            domain: message.Domain,
            senderDisplay: message.SenderDisplay,
            senderDomain: message.SenderDomain,
            recipientDisplay: message.RecipientDisplay,
            recipientDomain: message.RecipientDomain,
            messageId: message.MessageId,
            subject: message.Subject,
            subjectHash: message.SubjectHash,
            hasAttachments: message.HasAttachments,
            attachmentsCount: message.AttachmentsCount,
            size: message.Size,
            deliveryDecision: message.DeliveryDecision,
            dropReason: message.DropReason is not null
                ? new ProcessActivitiesCommandDropReasonDto
                {
                    Reason = message.DropReason.Reason,
                    Message = message.DropReason?.Message
                }
                : null,
            quarantineReason: message.QuarantineReason is not null
                ? new ProcessActivitiesCommandQuarantineReasonDto
                {
                    Reason = message.QuarantineReason.Reason,
                    Message = message.QuarantineReason?.Message
                }
                : null,
            forwardTo: message.ForwardTo,
            forwardFrom: message.ForwardFrom,
            provider: message.Provider,
            providerStatus: message.ProviderStatus,
            providerMessageId: message.ProviderMessageId,
            providerError: message.ProviderError,
            completedAt: message.CompletedAt,
            dedupKey: message.DedupKey,
            wait: 2500);
        await _mediator.Send(command, cancellationToken);
    }
}