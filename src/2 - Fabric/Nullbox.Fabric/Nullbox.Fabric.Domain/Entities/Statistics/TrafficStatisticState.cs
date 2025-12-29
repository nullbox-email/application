using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Statistics;

public partial class TrafficStatistic : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected TrafficStatistic()
    {
        Id = null!;
        BucketKey = null!;
    }

    public string Id { get; private set; }

    public Guid PartitionKey { get; private set; }

    public string BucketKey { get; private set; }

    public DateTimeOffset BucketStartAt { get; private set; }

    public int Total { get; private set; }

    public int Forwarded { get; private set; }

    public int Dropped { get; private set; }

    public int Quarantined { get; private set; }

    public int Delivered { get; private set; }

    public int Failed { get; private set; }

    public long Bandwidth { get; private set; }

    public int Aliases { get; private set; }

    public int Mailboxes { get; private set; }

    public string? LastCreatedEventId { get; private set; }

    public string? LastDecisionedEventId { get; private set; }

    public string? LastCompletedEventId { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}