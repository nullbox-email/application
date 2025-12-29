using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Dashboards;

public enum DashboardType
{
    Hourly,
    Daily
}