using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Application.Mailboxes;
using Nullbox.Fabric.Application.Mailboxes.CreateDefaultUserMailbox;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Accounts;

public class AccountCreateDefaultUserMailboxV1Handler : IIntegrationEventHandler<AccountCreateDefaultUserMailboxV1>
{
    private readonly ISender _mediator;

    public AccountCreateDefaultUserMailboxV1Handler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(
            AccountCreateDefaultUserMailboxV1 message,
            CancellationToken cancellationToken = default)
    {
        var command = new CreateDefaultUserMailboxCommand(
            id: message.Id,
            name: message.Name,
            domain: "nullbox.email",
            users: message.Users
                    .Select(u => new CreateDefaultUserMailboxCommandUsersDto
                    {
                        AccountId = u.AccountId,
                        UserProfileId = u.UserProfileId,
                        EmailAddress = u.EmailAddress,
                        RoleId = u.RoleId
                    })
                    .ToList());
        await _mediator.Send(command, cancellationToken);
    }
}