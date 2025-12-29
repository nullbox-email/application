using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Accounts.CreateAccount;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Security.Users.Eventing.Messages.Users;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Accounts;

public class UserProfileActivatedV1Handler : IIntegrationEventHandler<UserProfileActivatedV1Event>
{
    private readonly ISender _mediator;

    public UserProfileActivatedV1Handler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(UserProfileActivatedV1Event message, CancellationToken cancellationToken = default)
    {
        var command = new CreateAccountCommand(
            id: message.Id, name: message.Name, emailAddress: message.EmailAddress.NormalizedValue);

        var result = await _mediator.Send(command, cancellationToken);
    }
}