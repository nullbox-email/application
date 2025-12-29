using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Mailboxes;

public partial class MailboxMap : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected MailboxMap()
    {
        Id = null!;
        EmailAddress = null!;
    }

    /// <summary>
    /// Fully resolved RoutingKey + Domain
    /// </summary>
    public string Id { get; private set; }

    public Guid MailboxId { get; private set; }

    public Guid AccountId { get; private set; }

    public bool AutoCreateAlias { get; private set; } = true;

    public string EmailAddress { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}