using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.UpdateAlias;

public class UpdateAliasCommandValidator : AbstractValidator<UpdateAliasCommand>
{
    public UpdateAliasCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Name)
            .NotNull()
            .MaximumLength(128);
    }
}