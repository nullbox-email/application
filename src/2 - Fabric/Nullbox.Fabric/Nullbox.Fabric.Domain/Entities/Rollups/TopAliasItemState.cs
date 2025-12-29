using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Rollups;

public partial class TopAliasItem
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected TopAliasItem()
    {
        TopAliasId = null!;
    }

    public string TopAliasId { get; private set; }

    public Guid ParentId { get; private set; }

    public int Total { get; private set; }

    public int Forwarded { get; private set; }

    public int Dropped { get; private set; }

    public int Quarantined { get; private set; }

    public int Delivered { get; private set; }

    public int Failed { get; private set; }

    public long Bandwidth { get; private set; }
}