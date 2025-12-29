using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Mailboxes.Eventing.Messages.Mailboxes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Mailboxes;

public class MailboxUpdatedV1Handler : IIntegrationEventHandler<MailboxUpdatedV1Event>
{
    private readonly IMessageBus _messageBus;

    public MailboxUpdatedV1Handler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(MailboxUpdatedV1Event message, CancellationToken cancellationToken = default)
    {
        _messageBus.Send(new MailboxUpdateAliasMapV1
        {
            Id = message.Id,
            AccountId = message.AccountId,
            RoutingKey = message.RoutingKey,
            Name = message.Name,
            Domain = message.Domain,
            AutoCreateAlias = message.AutoCreateAlias,
            EmailAddress = message.EmailAddress
        });
        _messageBus.Send(new MailboxUpdateMailboxMapV1
        {
            Id = message.Id,
            AccountId = message.AccountId,
            RoutingKey = message.RoutingKey,
            Name = message.Name,
            Domain = message.Domain,
            AutoCreateAlias = message.AutoCreateAlias,
            EmailAddress = message.EmailAddress
        });
    }
}