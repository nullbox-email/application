using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.UpdateAliasMap;

public class UpdateAliasMapCommandValidator : AbstractValidator<UpdateAliasMapCommand>
{
    public UpdateAliasMapCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Id)
            .NotNull();

        RuleFor(v => v.Name)
            .NotNull();

        RuleFor(v => v.LocalPart)
            .NotNull();
    }
}