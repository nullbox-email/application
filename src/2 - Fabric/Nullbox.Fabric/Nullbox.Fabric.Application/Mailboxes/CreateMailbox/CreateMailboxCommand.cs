using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateMailbox;

public class CreateMailboxCommand : IRequest<string>, ICommand
{
    public CreateMailboxCommand(string name, bool autoCreateAlias, string emailAddress)
    {
        Name = name;
        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
    }

    public string Name { get; set; }
    public bool AutoCreateAlias { get; set; }
    public string EmailAddress { get; set; }
}