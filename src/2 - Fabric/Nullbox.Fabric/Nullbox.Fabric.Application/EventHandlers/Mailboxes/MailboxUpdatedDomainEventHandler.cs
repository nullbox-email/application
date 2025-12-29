using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Application.Common.Models;
using Nullbox.Fabric.Domain.Events.Mailboxes;
using Nullbox.Fabric.Mailboxes.Eventing.Messages.Mailboxes;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.EventHandlers.Mailboxes;

public class MailboxUpdatedDomainEventHandler : INotificationHandler<DomainEventNotification<MailboxUpdatedDomainEvent>>
{
    private readonly IMessageBus _messageBus;

    public MailboxUpdatedDomainEventHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task Handle(
            DomainEventNotification<MailboxUpdatedDomainEvent> notification,
            CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _messageBus.Publish(new MailboxUpdatedV1Event
        {
            Id = domainEvent.Mailbox.Id,
            AccountId = domainEvent.Mailbox.AccountId,
            RoutingKey = domainEvent.Mailbox.RoutingKey,
            Name = domainEvent.Mailbox.Name,
            Domain = domainEvent.Mailbox.Domain,
            AutoCreateAlias = domainEvent.Mailbox.AutoCreateAlias,
            EmailAddress = domainEvent.Mailbox.EmailAddress
        });
    }
}