using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateMailboxRoutingKeyMap;

public class CreateMailboxRoutingKeyMapCommandValidator : AbstractValidator<CreateMailboxRoutingKeyMapCommand>
{
    public CreateMailboxRoutingKeyMapCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.RoutingKey)
            .NotNull()
                .NotEmpty()
                .MaximumLength(32);
    }
}