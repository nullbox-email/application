using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Application.Common.Validation;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateDefaultUserMailbox;

public class CreateDefaultUserMailboxCommandValidator : AbstractValidator<CreateDefaultUserMailboxCommand>
{
    public CreateDefaultUserMailboxCommandValidator(IValidatorProvider provider)
    {
        ConfigureValidationRules(provider);
    }

    private void ConfigureValidationRules(IValidatorProvider provider)
    {
        RuleFor(v => v.Name)
            .NotNull();

        RuleFor(v => v.Domain)
            .NotNull()
                .MaximumLength(128);

        RuleFor(v => v.Users)
            .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateDefaultUserMailboxCommandUsersDto>()!));
    }
}