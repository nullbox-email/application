using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.GetAliases;

public class GetAliasesQueryValidator : AbstractValidator<GetAliasesQuery>
{
    public GetAliasesQueryValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.MailboxId)
            .NotNull();
    }
}