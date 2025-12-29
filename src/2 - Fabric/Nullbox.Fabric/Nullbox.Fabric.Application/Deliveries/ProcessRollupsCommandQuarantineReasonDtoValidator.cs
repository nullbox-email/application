using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Deliveries;

public class ProcessRollupsCommandQuarantineReasonDtoValidator : AbstractValidator<ProcessRollupsCommandQuarantineReasonDto>
{
    public ProcessRollupsCommandQuarantineReasonDtoValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Reason)
            .NotNull()
            .IsInEnum();
    }
}