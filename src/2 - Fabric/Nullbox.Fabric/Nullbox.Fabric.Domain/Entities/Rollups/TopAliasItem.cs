using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Rollups;

public partial class TopAliasItem
{
    public TopAliasItem(string mailboxTopAliasId,
            Guid parentId,
            int total,
            int forwarded,
            int dropped,
            int quarantined,
            int delivered,
            int failed,
            long bandwidth)
    {
        TopAliasId = mailboxTopAliasId;
        ParentId = parentId;
        Total = total;
        Forwarded = forwarded;
        Dropped = dropped;
        Quarantined = quarantined;
        Delivered = delivered;
        Failed = failed;
        Bandwidth = bandwidth;
    }

    public void Update(int total, int forwarded, int dropped, int quarantined, int delivered, int failed, long bandwidth)
    {
        Total = total;
        Forwarded = forwarded;
        Dropped = dropped;
        Quarantined = quarantined;
        Delivered = delivered;
        Failed = failed;
        Bandwidth = bandwidth;
    }
}