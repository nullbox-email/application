using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Mailboxes;

public partial class Mailbox : IHasDomainEvent
{
    private List<MailboxUser> _users = [];
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected Mailbox()
    {
        RoutingKey = null!;
        Name = null!;
        Domain = null!;
        EmailAddress = null!;
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public Guid AccountId { get; private set; }

    public string RoutingKey { get; private set; }

    public string Name { get; private set; }

    public string Domain { get; private set; }

    public bool AutoCreateAlias { get; private set; } = true;

    public string EmailAddress { get; private set; }

    public virtual IReadOnlyCollection<MailboxUser> Users
    {
        get => _users.AsReadOnly();
        private set => _users = new List<MailboxUser>(value);
    }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}