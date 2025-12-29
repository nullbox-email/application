using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Products;

public partial class ProductDefinition
{
    public ProductDefinition(string id,
            string name,
            bool isActive,
            int? maxMailboxes,
            int? maxAliases,
            int? maxAliasesPerMailbox,
            long? maxBandwidthBytesPerPeriod,
            Dictionary<string, string> flags)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        MaxMailboxes = maxMailboxes;
        MaxAliases = maxAliases;
        MaxAliasesPerMailbox = maxAliasesPerMailbox;
        MaxBandwidthBytesPerPeriod = maxBandwidthBytesPerPeriod;
        Flags = flags;
    }

    public void Update(
        string name,
        bool isActive,
        int? maxMailboxes,
        int? maxAliases,
        int? maxAliasesPerMailbox,
        long? maxBandwidthBytesPerPeriod,
        Dictionary<string, string> flags)
    {
        Name = name;
        IsActive = isActive;
        MaxMailboxes = maxMailboxes;
        MaxAliases = maxAliases;
        MaxAliasesPerMailbox = maxAliasesPerMailbox;
        MaxBandwidthBytesPerPeriod = maxBandwidthBytesPerPeriod;
        Flags = flags;
    }
}