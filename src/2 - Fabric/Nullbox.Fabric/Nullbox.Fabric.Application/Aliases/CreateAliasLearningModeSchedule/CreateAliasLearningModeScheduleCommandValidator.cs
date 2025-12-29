using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.CreateAliasLearningModeSchedule;

public class CreateAliasLearningModeScheduleCommandValidator : AbstractValidator<CreateAliasLearningModeScheduleCommand>
{
    public CreateAliasLearningModeScheduleCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Window)
            .NotNull()
            .MaximumLength(32);
    }
}