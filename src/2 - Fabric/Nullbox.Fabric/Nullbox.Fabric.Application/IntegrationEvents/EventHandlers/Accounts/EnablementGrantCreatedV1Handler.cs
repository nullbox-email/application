using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;
using Nullbox.Fabric.Application.Accounts.SynchronizeEffectiveAccountEnablement;
using Nullbox.Fabric.Application.Common.Eventing;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Accounts;

public class EnablementGrantCreatedV1Handler : IIntegrationEventHandler<EnablementGrantCreatedV1Event>
{
    private readonly ISender _mediator;

    public EnablementGrantCreatedV1Handler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task HandleAsync(EnablementGrantCreatedV1Event message, CancellationToken cancellationToken = default)
    {
        var command = new SynchronizeEffectiveAccountEnablementCommand(
            id: message.Id,
            accountId: message.AccountId,
            kind: message.Kind,
            productKey: message.ProductKey,
            priority: message.Priority,
            startsAt: message.StartsAt,
            endsAt: message.EndsAt,
            deltaMaxMailboxes: message.DeltaMaxMailboxes,
            deltaMaxAliases: message.DeltaMaxAliases,
            deltaMaxAliasesPerMailbox: message.DeltaMaxAliasesPerMailbox,
            deltaMaxBandwidthBytesPerPeriod: message.DeltaMaxBandwidthBytesPerPeriod,
            flags: message.Flags,
            source: message.Source,
            reason: message.Reason,
            isRevoked: message.IsRevoked);
        await _mediator.Send(command, cancellationToken);
    }
}