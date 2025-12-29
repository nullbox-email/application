using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Aliases.Eventing.Messages.Aliases;
using Nullbox.Fabric.Application.Aliases.CreateAliasMap;
using Nullbox.Fabric.Application.Common.Eventing;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Aliases;

public class CreateAliasMapV1Handler : IIntegrationEventHandler<CreateAliasMapV1>
{
    private readonly ISender _mediator;

    public CreateAliasMapV1Handler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(CreateAliasMapV1 message, CancellationToken cancellationToken = default)
    {
        var command = new CreateAliasMapCommand(
            id: message.Id,
            mailboxId: message.MailboxId,
            accountId: message.AccountId,
            name: message.Name,
            localPart: message.LocalPart, isEnabled: message.IsEnabled, directPassthrough: message.DirectPassthrough, learningMode: message.LearningMode);
        await _mediator.Send(command, cancellationToken);
    }
}