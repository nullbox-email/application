using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Aliases.Eventing.Messages.Aliases;
using Nullbox.Fabric.Application.Aliases.CreateAliasLearningModeSchedule;
using Nullbox.Fabric.Application.Common.Eventing;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.EventHandlers.Aliases;

public class AliasCreatedLearningModeScheduleV1Handler : IIntegrationEventHandler<AliasCreatedLearningModeScheduleV1>
{
    private readonly ISender _mediator;

    public AliasCreatedLearningModeScheduleV1Handler(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [IntentIgnore]
    public async Task HandleAsync(
        AliasCreatedLearningModeScheduleV1 message,
        CancellationToken cancellationToken = default)
    {
        // Learning window end: due in 30 days (UTC)
        var dueDateUtc = DateTimeOffset.UtcNow.AddDays(30);

        // Partition bucket: yyyyMMdd (UTC) so the daily Quartz job can scan a single partition.
        var window = Keys.DueDateBucket(dueDateUtc);

        var command = new CreateAliasLearningModeScheduleCommand(
            id: message.Id,
            window: window,
            aliasId: message.Id,
            mailboxId: message.MailboxId,
            dueDate: dueDateUtc);

        await _mediator.Send(command, cancellationToken);
    }

    private static class Keys
    {
        public static string DueDateBucket(DateTimeOffset dueDateUtc)
            => dueDateUtc.ToUniversalTime().ToString("yyyyMMdd");
    }
}
