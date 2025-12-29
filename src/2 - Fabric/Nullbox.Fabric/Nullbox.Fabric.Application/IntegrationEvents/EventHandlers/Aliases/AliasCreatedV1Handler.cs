using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Aliases.Eventing.Messages.Aliases;
using Nullbox.Fabric.Application.Common.Eventing;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Aliases;

public class AliasCreatedV1Handler : IIntegrationEventHandler<AliasCreatedV1Event>
{
    private readonly IMessageBus _messageBus;

    public AliasCreatedV1Handler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(AliasCreatedV1Event message, CancellationToken cancellationToken = default)
    {
        _messageBus.Send(new CreateAliasMapV1
        {
            Id = message.Id,
            MailboxId = message.MailboxId,
            AccountId = message.AccountId,
            Name = message.Name,
            LocalPart = message.LocalPart,
            IsEnabled = message.IsEnabled,
            DirectPassthrough = message.DirectPassthrough,
            LearningMode = message.LearningMode
        });
        _messageBus.Send(new AliasCreatedLearningModeScheduleV1
        {
            Id = message.Id,
            MailboxId = message.MailboxId,
            AccountId = message.AccountId,
            Name = message.Name,
            LocalPart = message.LocalPart,
            IsEnabled = message.IsEnabled,
            DirectPassthrough = message.DirectPassthrough,
            LearningMode = message.LearningMode
        });
    }
}