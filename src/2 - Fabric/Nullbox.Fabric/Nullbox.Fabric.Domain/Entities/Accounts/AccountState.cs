using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class Account : IHasDomainEvent
{
    private List<AccountUser> _users = [];

    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected Account()
    {
        Name = null!;
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public Guid AccountId { get; private set; }

    public string Name { get; private set; }

    public virtual IReadOnlyCollection<AccountUser> Users
    {
        get => _users.AsReadOnly();
        private set => _users = new List<AccountUser>(value);
    }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}