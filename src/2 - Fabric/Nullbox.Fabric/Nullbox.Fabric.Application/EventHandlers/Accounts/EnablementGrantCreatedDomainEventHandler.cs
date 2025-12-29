using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Application.Common.Models;
using Nullbox.Fabric.Domain.Events.Accounts;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.EventHandlers.Accounts;

public class EnablementGrantCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<EnablementGrantCreatedDomainEvent>>
{
    private readonly IMessageBus _messageBus;

    public EnablementGrantCreatedDomainEventHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task Handle(
DomainEventNotification<EnablementGrantCreatedDomainEvent> notification,
CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _messageBus.Publish(new EnablementGrantCreatedV1Event
        {
            Id = domainEvent.EnablementGrant.Id,
            AccountId = domainEvent.EnablementGrant.AccountId,
            Kind = domainEvent.EnablementGrant.Kind,
            ProductKey = domainEvent.EnablementGrant.ProductKey,
            Priority = domainEvent.EnablementGrant.Priority,
            StartsAt = domainEvent.EnablementGrant.StartsAt,
            EndsAt = domainEvent.EnablementGrant.EndsAt,
            DeltaMaxMailboxes = domainEvent.EnablementGrant.DeltaMaxMailboxes,
            DeltaMaxAliases = domainEvent.EnablementGrant.DeltaMaxAliases,
            DeltaMaxAliasesPerMailbox = domainEvent.EnablementGrant.DeltaMaxAliasesPerMailbox,
            DeltaMaxBandwidthBytesPerPeriod = domainEvent.EnablementGrant.DeltaMaxBandwidthBytesPerPeriod,
            Flags = domainEvent.EnablementGrant.Flags,
            Source = domainEvent.EnablementGrant.Source,
            Reason = domainEvent.EnablementGrant.Reason,
            IsRevoked = domainEvent.EnablementGrant.IsRevoked
        });
    }
}