using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Statistics;

public partial class TrafficStatistic
{
    public TrafficStatistic(string id,
        Guid partitionKey,
        string bucketKey,
        DateTimeOffset bucketStartAt,
        int total,
        int forwarded,
        int dropped,
        int quarantined,
        int delivered,
        int failed,
        long bandwidth,
        int aliases,
        int mailboxes,
        string? lastCreatedEventId,
        string? lastDecisionedEventId,
        string? lastCompletedEventId,
        DateTimeOffset updatedAt)
    {
        Id = id;
        PartitionKey = partitionKey;
        BucketKey = bucketKey;
        BucketStartAt = bucketStartAt;
        Total = total;
        Forwarded = forwarded;
        Dropped = dropped;
        Quarantined = quarantined;
        Delivered = delivered;
        Failed = failed;
        Bandwidth = bandwidth;
        Aliases = aliases;
        Mailboxes = mailboxes;
        LastCreatedEventId = lastCreatedEventId;
        LastDecisionedEventId = lastDecisionedEventId;
        LastCompletedEventId = lastCompletedEventId;
        UpdatedAt = updatedAt;
    }

    public void Update(
        int total,
        int forwarded,
        int dropped,
        int quarantined,
        int delivered,
        int failed,
        long bandwidth,
        int aliases,
        int mailboxes,
        string? lastCreatedEventId,
        string? lastDecisionedEventId,
        string? lastCompletedEventId,
        DateTimeOffset updatedAt)
    {
        Total = total;
        Forwarded = forwarded;
        Dropped = dropped;
        Quarantined = quarantined;
        Delivered = delivered;
        Failed = failed;
        Bandwidth = bandwidth;
        Aliases = aliases;
        Mailboxes = mailboxes;
        LastCreatedEventId = lastCreatedEventId;
        LastDecisionedEventId = lastDecisionedEventId;
        LastCompletedEventId = lastCompletedEventId;
        UpdatedAt = updatedAt;
    }
}