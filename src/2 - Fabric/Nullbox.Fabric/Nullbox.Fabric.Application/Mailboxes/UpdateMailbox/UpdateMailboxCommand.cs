using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.UpdateMailbox;

public class UpdateMailboxCommand : IRequest, ICommand
{
    public UpdateMailboxCommand(Guid id, string name, bool autoCreateAlias, string emailAddress)
    {
        Id = id;
        Name = name;
        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool AutoCreateAlias { get; set; }
    public string EmailAddress { get; set; }
}