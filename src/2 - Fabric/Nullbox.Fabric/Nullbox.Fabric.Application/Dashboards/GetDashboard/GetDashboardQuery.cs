using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Dashboards.GetDashboard;

public class GetDashboardQuery : IRequest<DashboardDto>, IQuery
{
    public GetDashboardQuery(Guid? aliasId, Guid? mailboxId, int number, DashboardType type = DashboardType.Daily)
    {
        AliasId = aliasId;
        MailboxId = mailboxId;
        Number = number;
        Type = type;
    }

    public Guid? AliasId { get; set; }
    public Guid? MailboxId { get; set; }
    public int Number { get; set; }
    public DashboardType Type { get; set; }
}