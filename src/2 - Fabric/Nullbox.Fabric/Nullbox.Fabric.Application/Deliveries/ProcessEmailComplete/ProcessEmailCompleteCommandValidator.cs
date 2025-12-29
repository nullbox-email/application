using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Deliveries.ProcessEmailComplete;

public class ProcessEmailCompleteCommandValidator : AbstractValidator<ProcessEmailCompleteCommand>
{
    public ProcessEmailCompleteCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {

        RuleFor(v => v.PartitionKey)
            .NotNull();

        RuleFor(v => v.Outcome)
            .NotNull();
    }
}