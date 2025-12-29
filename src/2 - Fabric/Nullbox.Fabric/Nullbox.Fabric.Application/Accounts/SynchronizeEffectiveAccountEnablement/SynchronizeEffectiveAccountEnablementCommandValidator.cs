using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Accounts.SynchronizeEffectiveAccountEnablement;

public class SynchronizeEffectiveAccountEnablementCommandValidator : AbstractValidator<SynchronizeEffectiveAccountEnablementCommand>
{
    public SynchronizeEffectiveAccountEnablementCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Kind)
            .NotNull()
            .IsInEnum();

        RuleFor(v => v.Flags)
            .NotNull();

        RuleFor(v => v.Source)
            .NotNull()
            .IsInEnum();
    }
}