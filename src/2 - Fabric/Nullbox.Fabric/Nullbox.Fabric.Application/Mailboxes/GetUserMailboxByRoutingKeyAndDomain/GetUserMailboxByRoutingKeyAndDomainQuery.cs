using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.GetUserMailboxByRoutingKeyAndDomain;

public class GetUserMailboxByRoutingKeyAndDomainQuery : IRequest<UserMailboxDto>, IQuery
{
    public GetUserMailboxByRoutingKeyAndDomainQuery(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}