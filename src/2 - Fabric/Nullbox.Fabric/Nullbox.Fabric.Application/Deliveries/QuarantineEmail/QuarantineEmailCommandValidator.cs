using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Deliveries.QuarantineEmail;

public class QuarantineEmailCommandValidator : AbstractValidator<QuarantineEmailCommand>
{
    public QuarantineEmailCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.PartitionKey)
            .NotNull();

        RuleFor(v => v.Reason)
            .NotNull()
                .IsInEnum();

        RuleFor(v => v.Message)
            .NotNull();

        RuleFor(v => v.Content)
            .NotNull();
    }
}