using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.AliasMapUpdateMailbox;

public class AliasMapUpdateMailboxCommandValidator : AbstractValidator<AliasMapUpdateMailboxCommand>
{
    public AliasMapUpdateMailboxCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.RoutingKey)
            .NotNull();

        RuleFor(v => v.Name)
            .NotNull();

        RuleFor(v => v.Domain)
            .NotNull();

        RuleFor(v => v.EmailAddress)
            .NotNull()
                .EmailAddress()
                .MaximumLength(320);
    }
}