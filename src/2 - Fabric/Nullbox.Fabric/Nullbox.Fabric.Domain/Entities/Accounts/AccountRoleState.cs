using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class AccountRole : IHasDomainEvent
{
    private List<string> _scopes = [];

    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected AccountRole()
    {
        Name = null!;
        Scopes = null!;
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public string Name { get; private set; }

    public IReadOnlyCollection<string> Scopes
    {
        get => _scopes.AsReadOnly();
        private set => _scopes = new List<string>(value);
    }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}