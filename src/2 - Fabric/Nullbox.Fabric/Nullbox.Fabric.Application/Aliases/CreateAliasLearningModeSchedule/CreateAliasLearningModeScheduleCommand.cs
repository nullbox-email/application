using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Aliases.CreateAliasLearningModeSchedule;

public class CreateAliasLearningModeScheduleCommand : IRequest, ICommand
{
    public CreateAliasLearningModeScheduleCommand(Guid id,
            string window,
            Guid aliasId,
            Guid mailboxId,
            DateTimeOffset dueDate)
    {
        Id = id;
        Window = window;
        AliasId = aliasId;
        MailboxId = mailboxId;
        DueDate = dueDate;
    }

    public Guid Id { get; set; }
    public string Window { get; set; }
    public Guid AliasId { get; set; }
    public Guid MailboxId { get; set; }
    public DateTimeOffset DueDate { get; set; }
}