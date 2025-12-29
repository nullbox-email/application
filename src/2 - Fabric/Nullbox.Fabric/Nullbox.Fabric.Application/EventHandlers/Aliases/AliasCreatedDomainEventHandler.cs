using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Aliases.Eventing.Messages.Aliases;
using Nullbox.Fabric.Application.Common.Eventing;
using Nullbox.Fabric.Application.Common.Models;
using Nullbox.Fabric.Domain.Events.Aliases;

[assembly: IntentTemplate("Intent.MediatR.DomainEvents.DomainEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.EventHandlers.Aliases;

public class AliasCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<AliasCreatedDomainEvent>>
{
    private readonly IMessageBus _messageBus;
    public AliasCreatedDomainEventHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task Handle(
            DomainEventNotification<AliasCreatedDomainEvent> notification,
            CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _messageBus.Publish(new AliasCreatedV1Event
        {
            Id = domainEvent.Alias.Id,
            MailboxId = domainEvent.Alias.MailboxId,
            AccountId = domainEvent.Alias.AccountId,
            Name = domainEvent.Alias.Name,
            LocalPart = domainEvent.Alias.LocalPart,
            IsEnabled = domainEvent.Alias.IsEnabled,
            DirectPassthrough = domainEvent.Alias.DirectPassthrough,
            LearningMode = domainEvent.Alias.LearningMode
        });
    }
}