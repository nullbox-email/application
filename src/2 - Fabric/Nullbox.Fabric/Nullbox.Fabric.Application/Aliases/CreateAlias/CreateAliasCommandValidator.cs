using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.CreateAlias;

public class CreateAliasCommandValidator : AbstractValidator<CreateAliasCommand>
{
    public CreateAliasCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Name)
            .NotNull()
            .MaximumLength(128);

        RuleFor(v => v.LocalPart)
            .NotNull()
            .MaximumLength(128);
    }
}