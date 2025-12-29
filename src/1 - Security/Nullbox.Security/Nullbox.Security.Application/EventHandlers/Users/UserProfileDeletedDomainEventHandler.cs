using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Eventing;
using Nullbox.Security.Application.Common.Models;
using Nullbox.Security.Domain.Events.Users;
using Nullbox.Security.Users.Eventing.Messages.Users;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Nullbox.Security.Application.EventHandlers.Users;

public class UserProfileDeletedDomainEventHandler : INotificationHandler<DomainEventNotification<UserProfileDeletedDomainEvent>>
{
    private readonly IMessageBus _messageBus;
    public UserProfileDeletedDomainEventHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task Handle(
            DomainEventNotification<UserProfileDeletedDomainEvent> notification,
            CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _messageBus.Publish(new UserProfileDeletedV1Event
        {
            Id = domainEvent.UserProfile.Id,
            Name = domainEvent.UserProfile.Name,
            EmailAddress = new UserProfileDeletedV1EmailAddressDto
            {
                Value = domainEvent.UserProfile.EmailAddress.Value,
                NormalizedValue = domainEvent.UserProfile.EmailAddress.NormalizedValue
            },
            Status = domainEvent.UserProfile.Status
        });
    }
}