using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Dashboards;

public class DashboardDto
{
    public DashboardDto()
    {
        Chart = null!;
        Messages = null!;
    }

    public List<TrafficStatisticDto> Chart { get; set; }
    public List<DeliveryActionDto> Messages { get; set; }

    public static DashboardDto Create(List<TrafficStatisticDto> chart, List<DeliveryActionDto> messages)
    {
        return new DashboardDto
        {
            Chart = chart,
            Messages = messages
        };
    }
}