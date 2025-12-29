using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Rollups;

public partial class TopAlias
{
    public TopAlias(string id,
        Guid partitionKey,
        string windowKey,
        DateTimeOffset windowStart,
        DateTimeOffset windowEnd,
        DateTimeOffset updatedAt)
    {
        Id = id;
        PartitionKey = partitionKey;
        WindowKey = windowKey;
        WindowStart = windowStart;
        WindowEnd = windowEnd;
        UpdatedAt = updatedAt;
    }

    public void UpdateItem(
        Guid parentId,
        int total,
        int forwarded,
        int dropped,
        int quarantined,
        int delivered,
        int failed,
        long bandwidth)
    {
        var existingItem = Items.FirstOrDefault(x => x.ParentId == parentId);

        if (existingItem != null)
        {
            existingItem.Update(total, forwarded, dropped, quarantined, delivered, failed, bandwidth);
        }
        else
        {
            var newItem = new TopAliasItem(
                mailboxTopAliasId: Guid.NewGuid().ToString(),
                parentId: parentId,
                total: total,
                forwarded: forwarded,
                dropped: dropped,
                quarantined: quarantined,
                delivered: delivered,
                failed: failed,
                bandwidth: bandwidth
            );

            _items.Add(newItem);
        }

        UpdatedAt = DateTimeOffset.UtcNow;
    }
}