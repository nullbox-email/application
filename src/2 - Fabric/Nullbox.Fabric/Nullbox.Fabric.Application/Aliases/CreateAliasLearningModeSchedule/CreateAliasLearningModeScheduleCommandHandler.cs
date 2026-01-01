using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Entities.Aliases;
using Nullbox.Fabric.Domain.Repositories.Aliases;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.CreateAliasLearningModeSchedule;

public class CreateAliasLearningModeScheduleCommandHandler : IRequestHandler<CreateAliasLearningModeScheduleCommand>
{
    private readonly IAliasLearningModeScheduleRepository _aliasLearningModeScheduleRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public CreateAliasLearningModeScheduleCommandHandler(
        IAliasLearningModeScheduleRepository aliasLearningModeScheduleRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _aliasLearningModeScheduleRepository = aliasLearningModeScheduleRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    public async Task Handle(CreateAliasLearningModeScheduleCommand request, CancellationToken cancellationToken)
    {
        using var _ = _partitionKeyScope.Push(request.Window);

        var aliasLearningModeSchedule = new AliasLearningModeSchedule(
            id: request.Id,
            window: request.Window,
            aliasId: request.AliasId, mailboxId: request.MailboxId, dueDate: request.DueDate);

        _aliasLearningModeScheduleRepository.Add(aliasLearningModeSchedule);
    }
}