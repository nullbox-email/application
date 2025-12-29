using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Aliases.CreateAlias;

public class CreateAliasCommand : IRequest<string>, ICommand
{
    public CreateAliasCommand(Guid mailboxId, string name, string localPart)
    {
        MailboxId = mailboxId;
        Name = name;
        LocalPart = localPart;
    }

    public Guid MailboxId { get; set; }
    public string Name { get; set; }
    public string LocalPart { get; set; }
}