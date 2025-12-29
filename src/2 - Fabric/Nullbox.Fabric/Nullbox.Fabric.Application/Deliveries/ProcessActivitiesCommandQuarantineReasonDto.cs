using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Deliveries;

public class ProcessActivitiesCommandQuarantineReasonDto
{
    public ProcessActivitiesCommandQuarantineReasonDto()
    {
    }

    public QuarantineReason Reason { get; set; }
    public string? Message { get; set; }

    public static ProcessActivitiesCommandQuarantineReasonDto Create(QuarantineReason reason, string? message)
    {
        return new ProcessActivitiesCommandQuarantineReasonDto
        {
            Reason = reason,
            Message = message
        };
    }
}