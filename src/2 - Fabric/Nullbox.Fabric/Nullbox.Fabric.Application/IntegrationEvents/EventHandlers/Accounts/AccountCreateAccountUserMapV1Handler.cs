using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;
using Nullbox.Fabric.Application.Accounts.CreateAccountUserMap;
using Nullbox.Fabric.Application.Common.Eventing;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Accounts;

public class AccountCreateAccountUserMapV1Handler : IIntegrationEventHandler<AccountCreateAccountUserMapV1>
{
    private readonly ISender _mediator;

    public AccountCreateAccountUserMapV1Handler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(AccountCreateAccountUserMapV1 message, CancellationToken cancellationToken = default)
    {
        var command = new CreateAccountUserMapCommand(
            id: message.Id,
            partitionKey: message.UserId);
        await _mediator.Send(command, cancellationToken);
    }
}