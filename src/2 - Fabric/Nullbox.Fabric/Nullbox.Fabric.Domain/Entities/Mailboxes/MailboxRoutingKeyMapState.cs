using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Mailboxes;

public partial class MailboxRoutingKeyMap : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected MailboxRoutingKeyMap()
    {
        Id = null!;
    }

    /// <summary>
    /// RoutingKey
    /// </summary>
    public string Id { get; private set; }

    public Guid MailboxId { get; private set; }

    public Guid AccountId { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}