using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Domain.Entities.Aliases;
using Nullbox.Fabric.Domain.Repositories.Aliases;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.CreateAliasLearningModeSchedule;

public class CreateAliasLearningModeScheduleCommandHandler : IRequestHandler<CreateAliasLearningModeScheduleCommand>
{
    private readonly IAliasLearningModeScheduleRepository _aliasLearningModeScheduleRepository;

    public CreateAliasLearningModeScheduleCommandHandler(IAliasLearningModeScheduleRepository aliasLearningModeScheduleRepository)
    {
        _aliasLearningModeScheduleRepository = aliasLearningModeScheduleRepository;
    }

    public async Task Handle(CreateAliasLearningModeScheduleCommand request, CancellationToken cancellationToken)
    {
        var aliasLearningModeSchedule = new AliasLearningModeSchedule(
            id: request.Id,
            window: request.Window,
            aliasId: request.AliasId, mailboxId: request.MailboxId, dueDate: request.DueDate);

        _aliasLearningModeScheduleRepository.Add(aliasLearningModeSchedule);
    }
}