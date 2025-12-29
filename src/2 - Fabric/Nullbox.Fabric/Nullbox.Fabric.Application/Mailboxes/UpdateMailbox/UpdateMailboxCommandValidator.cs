using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.UpdateMailbox;

public class UpdateMailboxCommandValidator : AbstractValidator<UpdateMailboxCommand>
{
    public UpdateMailboxCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Name)
            .NotNull()
            .MaximumLength(64);

        RuleFor(v => v.EmailAddress)
            .NotNull()
                .EmailAddress()
                .MaximumLength(320);
    }
}