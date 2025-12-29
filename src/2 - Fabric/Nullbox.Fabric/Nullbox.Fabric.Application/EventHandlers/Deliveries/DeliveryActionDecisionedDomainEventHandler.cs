using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Application.Common.Models;
using Nullbox.Fabric.Deliveries.Eventing.Messages.Deliveries;
using Nullbox.Fabric.Domain.Events.Deliveries;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.EventHandlers.Deliveries;

public class DeliveryActionDecisionedDomainEventHandler : INotificationHandler<DomainEventNotification<DeliveryActionDecisionedDomainEvent>>
{
    private readonly IMessageBus _messageBus;

    public DeliveryActionDecisionedDomainEventHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task Handle(
            DomainEventNotification<DeliveryActionDecisionedDomainEvent> notification,
            CancellationToken cancellationToken)
    {
        //var domainEvent = notification.DomainEvent;
        //_messageBus.Publish(new DeliveryActionDecisionedV1Event
        //{
        //    Id = domainEvent.DeliveryAction.Id,
        //    PartitionKey = domainEvent.DeliveryAction.PartitionKey,
        //    AliasId = domainEvent.DeliveryAction.AliasId,
        //    MailboxId = domainEvent.DeliveryAction.MailboxId,
        //    AccountId = domainEvent.DeliveryAction.AccountId,
        //    Source = domainEvent.DeliveryAction.Source,
        //    CreatedAt = domainEvent.DeliveryAction.CreatedAt,
        //    ReceivedAt = domainEvent.DeliveryAction.ReceivedAt,
        //    Alias = domainEvent.DeliveryAction.Alias,
        //    RoutingKey = domainEvent.DeliveryAction.RoutingKey,
        //    Domain = domainEvent.DeliveryAction.Domain,
        //    SenderDisplay = domainEvent.DeliveryAction.SenderDisplay,
        //    SenderDomain = domainEvent.DeliveryAction.SenderDomain,
        //    RecipientDisplay = domainEvent.DeliveryAction.RecipientDisplay,
        //    RecipientDomain = domainEvent.DeliveryAction.RecipientDomain,
        //    MessageId = domainEvent.DeliveryAction.MessageId,
        //    Subject = domainEvent.DeliveryAction.Subject,
        //    SubjectHash = domainEvent.DeliveryAction.SubjectHash,
        //    HasAttachments = domainEvent.DeliveryAction.HasAttachments,
        //    AttachmentsCount = domainEvent.DeliveryAction.AttachmentsCount,
        //    Size = domainEvent.DeliveryAction.Size,
        //    DeliveryDecision = domainEvent.DeliveryAction.DeliveryDecision,
        //    DropReason = domainEvent.DeliveryAction.DropReason is not null
        //            ? new DeliveryActionDecisionedV1DropReasonDto
        //            {
        //                Reason = domainEvent.DeliveryAction.DropReason.Reason,
        //                Message = domainEvent.DeliveryAction.DropReason?.Message
        //            }
        //            : null,
        //    QuarantineReason = domainEvent.DeliveryAction.QuarantineReason is not null
        //            ? new DeliveryActionDecisionedV1QuarantineReasonDto
        //            {
        //                Reason = domainEvent.DeliveryAction.QuarantineReason.Reason,
        //                Message = domainEvent.DeliveryAction.QuarantineReason?.Message
        //            }
        //            : null,
        //    ForwardTo = domainEvent.DeliveryAction.ForwardTo,
        //    ForwardFrom = domainEvent.DeliveryAction.ForwardFrom,
        //    Provider = domainEvent.DeliveryAction.Provider,
        //    ProviderStatus = domainEvent.DeliveryAction.ProviderStatus,
        //    ProviderMessageId = domainEvent.DeliveryAction.ProviderMessageId,
        //    ProviderError = domainEvent.DeliveryAction.ProviderError,
        //    CompletedAt = domainEvent.DeliveryAction.CompletedAt,
        //    DedupKey = domainEvent.DeliveryAction.DedupKey
        //});
    }
}