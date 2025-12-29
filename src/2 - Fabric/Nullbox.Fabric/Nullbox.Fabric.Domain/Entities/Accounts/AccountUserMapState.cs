using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class AccountUserMap : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected AccountUserMap()
    {
    }
    public Guid Id { get; private set; }

    public Guid PartitionKey { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}