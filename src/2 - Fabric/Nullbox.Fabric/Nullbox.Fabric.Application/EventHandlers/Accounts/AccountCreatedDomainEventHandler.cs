using Azure.Core;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Common.Models;
using Nullbox.Fabric.Domain.Events.Accounts;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.EventHandlers.Accounts;

public class AccountCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<AccountCreatedDomainEvent>>
{
    private readonly IMessageBus _messageBus;

    public AccountCreatedDomainEventHandler(
        IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task Handle(
            DomainEventNotification<AccountCreatedDomainEvent> notification,
            CancellationToken cancellationToken)
    {
        var defaultUserProfile = notification.DomainEvent.Account.Users.First();

        var domainEvent = notification.DomainEvent;
        _messageBus.Publish(new AccountCreatedV1Event
        {
            Id = domainEvent.Account.Id,
            // [IntentIgnore]
            UserId = defaultUserProfile.UserProfileId,
            Name = domainEvent.Account.Name,
            Users = domainEvent.Account.Users
                    .Select(u => new AccountCreatedV1UsersDto
                    {
                        AccountId = u.AccountId,
                        UserProfileId = u.UserProfileId,
                        EmailAddress = u.EmailAddress,
                        RoleId = u.RoleId
                    })
                    .ToList()
        });
    }
}