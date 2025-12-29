using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;
using Nullbox.Fabric.Application.Accounts.CreateEnablementGrant;
using Nullbox.Fabric.Application.Common.Eventing;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Accounts;

public class AccountCreateEnablementV1Handler : IIntegrationEventHandler<AccountCreateEnablementV1>
{
    private readonly ISender _mediator;

    public AccountCreateEnablementV1Handler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(AccountCreateEnablementV1 message, CancellationToken cancellationToken = default)
    {
        var command = new CreateEnablementGrantCommand(
            accountId: message.Id);
        await _mediator.Send(command, cancellationToken);
    }
}