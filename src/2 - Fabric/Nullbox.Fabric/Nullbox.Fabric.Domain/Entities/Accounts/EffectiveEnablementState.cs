using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class EffectiveEnablement : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected EffectiveEnablement()
    {
        EffectiveProductKey = null!;
    }

    public Guid Id { get; private set; }

    public Guid AccountId { get; private set; }

    public string EffectiveProductKey { get; private set; }

    public int? MaxMailboxes { get; private set; }

    public int? MaxAliases { get; private set; }

    public int? MaxAliasesPerMailbox { get; private set; }

    public long? MaxBandwidthBytesPerPeriod { get; private set; }

    public Dictionary<string, string> Flags { get; private set; } = [];

    public List<DomainEvent> DomainEvents { get; set; } = [];
}