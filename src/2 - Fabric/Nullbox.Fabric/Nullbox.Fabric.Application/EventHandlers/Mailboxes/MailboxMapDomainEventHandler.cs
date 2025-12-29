using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Models;
using Nullbox.Fabric.Application.Mailboxes.CreateMailboxMap;
using Nullbox.Fabric.Domain.Events.Mailboxes;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.EventHandlers.Mailboxes;

public class MailboxMapDomainEventHandler : INotificationHandler<DomainEventNotification<MailboxCreatedDomainEvent>>
{
    private readonly ISender _mediator;

    public MailboxMapDomainEventHandler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Handle(
            DomainEventNotification<MailboxCreatedDomainEvent> notification,
            CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var command = new CreateMailboxMapCommand(
            // [IntentIgnore]
            id: $"{domainEvent.Mailbox.RoutingKey}.{domainEvent.Mailbox.Domain}",
            mailboxId: domainEvent.Mailbox.Id,
            userId: domainEvent.Mailbox.AccountId,
            autoCreateAlias: domainEvent.Mailbox.AutoCreateAlias,
            emailAddress: domainEvent.Mailbox.EmailAddress);
        await _mediator.Send(command, cancellationToken);
    }
}