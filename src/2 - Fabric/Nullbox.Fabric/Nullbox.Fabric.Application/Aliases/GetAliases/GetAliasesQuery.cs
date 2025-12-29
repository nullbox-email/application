using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Aliases.GetAliases;

public class GetAliasesQuery : IRequest<List<AliasDto>>, IQuery
{
    public GetAliasesQuery(string mailboxId)
    {
        MailboxId = mailboxId;
    }

    public string MailboxId { get; set; }
}