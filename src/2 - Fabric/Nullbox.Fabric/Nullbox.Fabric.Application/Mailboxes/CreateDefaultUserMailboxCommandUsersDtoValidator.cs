using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.DTOValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes;

public class CreateDefaultUserMailboxCommandUsersDtoValidator : AbstractValidator<CreateDefaultUserMailboxCommandUsersDto>
{
    public CreateDefaultUserMailboxCommandUsersDtoValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.EmailAddress)
            .NotNull();
    }
}