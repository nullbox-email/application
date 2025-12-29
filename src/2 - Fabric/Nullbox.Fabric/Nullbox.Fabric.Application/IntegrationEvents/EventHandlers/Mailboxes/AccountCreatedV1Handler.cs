using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;
using Nullbox.Fabric.Application.Common.Eventing;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Mailboxes;

public class AccountCreatedV1Handler : IIntegrationEventHandler<AccountCreatedV1Event>
{
    private readonly IMessageBus _messageBus;

    public AccountCreatedV1Handler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(AccountCreatedV1Event message, CancellationToken cancellationToken = default)
    {
        _messageBus.Send(new AccountCreateAccountUserMapV1
        {
            Id = message.Id,
            UserId = message.UserId,
            Name = message.Name,
            Users = message.Users
                    .Select(u => new AccountCreatedV1UsersDto
                    {
                        AccountId = u.AccountId,
                        UserProfileId = u.UserProfileId,
                        EmailAddress = u.EmailAddress,
                        RoleId = u.RoleId
                    })
                    .ToList()
        });
        _messageBus.Send(new AccountCreateDefaultUserMailboxV1
        {
            Id = message.Id,
            UserId = message.UserId,
            Name = message.Name,
            Users = message.Users
                    .Select(u => new AccountCreatedV1UsersDto
                    {
                        AccountId = u.AccountId,
                        UserProfileId = u.UserProfileId,
                        EmailAddress = u.EmailAddress,
                        RoleId = u.RoleId
                    })
                    .ToList()
        });
        _messageBus.Send(new AccountCreateEnablementV1
        {
            Id = message.Id,
            UserId = message.UserId,
            Name = message.Name,
            Users = message.Users
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