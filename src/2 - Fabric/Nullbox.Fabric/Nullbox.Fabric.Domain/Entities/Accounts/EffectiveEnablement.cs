using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class EffectiveEnablement
{
    public EffectiveEnablement(Guid id,
        string effectiveProductKey,
        int? maxMailboxes,
        int? maxAliases,
        int? maxAliasesPerMailbox,
        long? maxBandwidthBytesPerPeriod,
        Dictionary<string, string> flags)
    {
        Id = id;
        AccountId = id;
        EffectiveProductKey = effectiveProductKey;
        MaxMailboxes = maxMailboxes;
        MaxAliases = maxAliases;
        MaxAliasesPerMailbox = maxAliasesPerMailbox;
        MaxBandwidthBytesPerPeriod = maxBandwidthBytesPerPeriod;
        Flags = flags;
    }

    public void Update(
        string effectiveProductKey,
        int? maxMailboxes,
        int? maxAliases,
        int? maxAliasesPerMailbox,
        long? maxBandwidthBytesPerPeriod,
        Dictionary<string, string> flags)
    {
        EffectiveProductKey = effectiveProductKey;
        MaxMailboxes = maxMailboxes;
        MaxAliases = maxAliases;
        MaxAliasesPerMailbox = maxAliasesPerMailbox;
        MaxBandwidthBytesPerPeriod = maxBandwidthBytesPerPeriod;
        Flags = flags;
    }
}