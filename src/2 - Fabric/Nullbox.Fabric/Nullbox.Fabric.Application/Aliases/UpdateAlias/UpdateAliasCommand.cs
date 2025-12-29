using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Aliases.UpdateAlias;

public class UpdateAliasCommand : IRequest, ICommand
{
    public UpdateAliasCommand(Guid id, Guid mailboxId, string name, bool isEnabled, bool directPassthrough, bool learningMode)
    {
        Id = id;
        MailboxId = mailboxId;
        Name = name;
        IsEnabled = isEnabled;
        DirectPassthrough = directPassthrough;
        LearningMode = learningMode;
    }

    public Guid Id { get; set; }
    public Guid MailboxId { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public bool DirectPassthrough { get; set; }
    public bool LearningMode { get; set; }
}