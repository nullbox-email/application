using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Models;
using Nullbox.Fabric.Application.Mailboxes.CreateMailboxRoutingKeyMap;
using Nullbox.Fabric.Domain.Events.Mailboxes;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.EventHandlers.Mailboxes;

public class MailboxRoutingKeyMapDomainEventHandler : INotificationHandler<DomainEventNotification<MailboxCreatedDomainEvent>>
{
    private readonly ISender _mediator;

    public MailboxRoutingKeyMapDomainEventHandler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Handle(
            DomainEventNotification<MailboxCreatedDomainEvent> notification,
            CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var command = new CreateMailboxRoutingKeyMapCommand(
            routingKey: domainEvent.Mailbox.RoutingKey,
            id: domainEvent.Mailbox.Id,
            userId: domainEvent.Mailbox.AccountId);
        await _mediator.Send(command, cancellationToken);
    }
}