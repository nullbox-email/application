using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Deliveries.ProcessEmail;

public class ProcessEmailCommandValidator : AbstractValidator<ProcessEmailCommand>
{
    public ProcessEmailCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {

        RuleFor(v => v.Alias)
            .NotNull()
                .NotEmpty();

        RuleFor(v => v.RoutingKey)
            .NotNull()
                .NotEmpty();

        RuleFor(v => v.Domain)
            .NotNull()
                .NotEmpty();

        RuleFor(v => v.Sender)
            .NotNull()
                .NotEmpty()
            .EmailAddress();

        RuleFor(v => v.SenderDomain)
            .NotNull();

        RuleFor(v => v.Recipient)
            .NotNull()
                .NotEmpty()
                .EmailAddress();

        RuleFor(v => v.RecipientDomain)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.Subject)
            .NotNull();

        RuleFor(v => v.SubjectHash)
            .NotNull()
            .NotEmpty();

        RuleFor(v => v.Source)
            .NotNull()
            .NotEmpty();
    }
}