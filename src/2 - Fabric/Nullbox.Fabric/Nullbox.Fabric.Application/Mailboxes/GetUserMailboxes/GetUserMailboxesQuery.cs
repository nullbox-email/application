using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.GetUserMailboxes;

public class GetUserMailboxesQuery : IRequest<List<MailboxDto>>, IQuery
{
    public GetUserMailboxesQuery()
    {
    }
}