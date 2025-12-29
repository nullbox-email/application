using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateMailboxMap;

public class CreateMailboxMapCommandValidator : AbstractValidator<CreateMailboxMapCommand>
{
    public CreateMailboxMapCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Id)
            .NotNull()
            .MaximumLength(160);

        RuleFor(v => v.EmailAddress)
            .NotNull()
                .EmailAddress()
                .MaximumLength(320);
    }
}