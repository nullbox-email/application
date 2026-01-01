using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Dashboards;

public class TrafficStatisticDto
{
    public TrafficStatisticDto()
    {
    }

    public DateTimeOffset Timestamp { get; set; }
    public int Total { get; set; }
    public int Forwarded { get; set; }
    public int Dropped { get; set; }
    public int Quarantined { get; set; }
    public int Delivered { get; set; }
    public int Failed { get; set; }
    public long Bandwidth { get; set; }

    public static TrafficStatisticDto Create(
        DateTimeOffset timestamp,
        int total,
        int forwarded,
        int dropped,
        int quarantined,
        int delivered,
        int failed,
        long bandwidth)
    {
        return new TrafficStatisticDto
        {
            Timestamp = timestamp,
            Total = total,
            Forwarded = forwarded,
            Dropped = dropped,
            Quarantined = quarantined,
            Delivered = delivered,
            Failed = failed,
            Bandwidth = bandwidth
        };
    }
}