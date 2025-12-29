using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Rollups;

public partial class TopDomain : IHasDomainEvent
{
    private List<TopDomainItem> _items = [];

    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected TopDomain()
    {
        Id = null!;
        WindowKey = null!;
    }

    public string Id { get; private set; }

    public string WindowKey { get; private set; }

    public DateTimeOffset WindowStart { get; private set; }

    public DateTimeOffset WindowEnd { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public virtual IReadOnlyCollection<TopDomainItem> Items
    {
        get => _items.AsReadOnly();
        private set => _items = new List<TopDomainItem>(value);
    }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}