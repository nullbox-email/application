using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Eventing;
using Nullbox.Security.Application.Common.Models;
using Nullbox.Security.Domain.Events.Users;
using Nullbox.Security.Users.Eventing.Messages.Users;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Nullbox.Security.Application.EventHandlers.Users;

public class ExternalUserCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<ExternalUserCreatedDomainEvent>>
{
    private readonly IMessageBus _messageBus;
    public ExternalUserCreatedDomainEventHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task Handle(
            DomainEventNotification<ExternalUserCreatedDomainEvent> notification,
            CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _messageBus.Publish(new ExternalUserCreatedV1Event
        {
            Id = domainEvent.ExternalUser.Id,
            Context = domainEvent.ExternalUser.Context,
            UserId = domainEvent.ExternalUser.UserId
        });
    }
}