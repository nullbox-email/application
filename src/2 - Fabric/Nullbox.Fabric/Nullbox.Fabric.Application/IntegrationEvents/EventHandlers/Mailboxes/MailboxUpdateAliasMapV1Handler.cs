using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Application.Mailboxes.AliasMapUpdateMailbox;
using Nullbox.Fabric.Mailboxes.Eventing.Messages.Mailboxes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Mailboxes;

public class MailboxUpdateAliasMapV1Handler : IIntegrationEventHandler<MailboxUpdateAliasMapV1>
{
    private readonly ISender _mediator;

    public MailboxUpdateAliasMapV1Handler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(MailboxUpdateAliasMapV1 message, CancellationToken cancellationToken = default)
    {
        var command = new AliasMapUpdateMailboxCommand(
            id: message.Id,
            accountId: message.AccountId,
            routingKey: message.RoutingKey,
            name: message.Name,
            domain: message.Domain,
            autoCreateAlias: message.AutoCreateAlias,
            emailAddress: message.EmailAddress);
        await _mediator.Send(command, cancellationToken);
    }
}