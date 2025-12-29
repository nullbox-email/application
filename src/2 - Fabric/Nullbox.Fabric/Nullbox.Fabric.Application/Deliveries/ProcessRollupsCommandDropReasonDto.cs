using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Deliveries;

public class ProcessRollupsCommandDropReasonDto
{
    public ProcessRollupsCommandDropReasonDto()
    {
    }

    public DropReason Reason { get; set; }
    public string? Message { get; set; }

    public static ProcessRollupsCommandDropReasonDto Create(DropReason reason, string? message)
    {
        return new ProcessRollupsCommandDropReasonDto
        {
            Reason = reason,
            Message = message
        };
    }
}