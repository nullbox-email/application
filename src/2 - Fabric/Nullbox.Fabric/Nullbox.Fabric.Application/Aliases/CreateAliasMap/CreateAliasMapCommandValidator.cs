using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.CreateAliasMap;

public class CreateAliasMapCommandValidator : AbstractValidator<CreateAliasMapCommand>
{
    public CreateAliasMapCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Name)
            .NotNull();

        RuleFor(v => v.LocalPart)
            .NotNull();
    }
}