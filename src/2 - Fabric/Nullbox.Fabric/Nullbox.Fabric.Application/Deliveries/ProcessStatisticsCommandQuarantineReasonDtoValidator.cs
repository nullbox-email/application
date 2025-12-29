using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Deliveries;

public class ProcessStatisticsCommandQuarantineReasonDtoValidator : AbstractValidator<ProcessStatisticsCommandQuarantineReasonDto>
{
    public ProcessStatisticsCommandQuarantineReasonDtoValidator()
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