using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Aliases.GetAliasById;

public class GetAliasByIdQuery : IRequest<AliasDto>, IQuery
{
    public GetAliasByIdQuery(string id, string mailboxId)
    {
        Id = id;
        MailboxId = mailboxId;
    }

    public string Id { get; set; }
    public string MailboxId { get; set; }
}