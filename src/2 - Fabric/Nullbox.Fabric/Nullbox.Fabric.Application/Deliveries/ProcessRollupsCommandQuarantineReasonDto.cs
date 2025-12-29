using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Deliveries;

public class ProcessRollupsCommandQuarantineReasonDto
{
    public ProcessRollupsCommandQuarantineReasonDto()
    {
    }

    public QuarantineReason Reason { get; set; }
    public string? Message { get; set; }

    public static ProcessRollupsCommandQuarantineReasonDto Create(QuarantineReason reason, string? message)
    {
        return new ProcessRollupsCommandQuarantineReasonDto
        {
            Reason = reason,
            Message = message
        };
    }
}