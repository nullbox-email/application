using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Rollups;

public partial class TopDomainItem
{
    public TopDomainItem(string systemTopTargetDomainId,
            string domain,
            int total,
            int forwarded,
            int dropped,
            int quarantined,
            int delivered,
            int failed,
            long bandwidth)
    {
        TopTargetDomainId = systemTopTargetDomainId;
        Domain = domain;
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