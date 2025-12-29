using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Products;

public partial class ProductDefinition : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected ProductDefinition()
    {
        Id = null!;
        Name = null!;
    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; } = true;

    public int? MaxMailboxes { get; private set; }

    public int? MaxAliases { get; private set; }

    public int? MaxAliasesPerMailbox { get; private set; }

    public long? MaxBandwidthBytesPerPeriod { get; private set; }

    public Dictionary<string, string> Flags { get; private set; } = [];

    public List<DomainEvent> DomainEvents { get; set; } = [];
}