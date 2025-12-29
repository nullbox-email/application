using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.GetAliasById;

public class GetAliasByIdQueryValidator : AbstractValidator<GetAliasByIdQuery>
{
    public GetAliasByIdQueryValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Id)
            .NotNull();

        RuleFor(v => v.MailboxId)
            .NotNull();
    }
}