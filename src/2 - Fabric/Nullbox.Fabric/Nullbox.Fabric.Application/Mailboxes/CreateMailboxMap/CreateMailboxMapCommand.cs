using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateMailboxMap;

public class CreateMailboxMapCommand : IRequest, ICommand
{
    public CreateMailboxMapCommand(string id, Guid mailboxId, Guid userId, bool autoCreateAlias, string emailAddress)
    {
        Id = id;
        MailboxId = mailboxId;
        UserId = userId;
        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
    }

    public string Id { get; set; }
    public Guid MailboxId { get; set; }
    public Guid UserId { get; set; }
    public bool AutoCreateAlias { get; set; }
    public string EmailAddress { get; set; }
}