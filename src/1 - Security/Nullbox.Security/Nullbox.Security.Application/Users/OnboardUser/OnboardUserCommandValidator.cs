using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Security.Application.Users.OnboardUser;

public class OnboardUserCommandValidator : AbstractValidator<OnboardUserCommand>
{
    public OnboardUserCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Name)
            .NotNull()
            .NotEmpty()
            .Length(3, 64)
            .CustomAsync(ValidateNameAsync);

        RuleFor(v => v.RemoteIp)
            .NotNull();

        RuleFor(v => v.CfTurnstyleResponse)
            .NotNull();
    }

    private readonly HashSet<string> _blockedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        // System / Roles
        "admin", "administrator", "root", "system", "support", "helpdesk",
        "mod", "moderator", "staff", "owner",

        // Product impersonation
        "nullbox", "ni team", "ni support", "official", "verified",

        // Religious figures
        "god", "allah", "buddha", "messiah", "prophet",

        // Profanity placeholders
        "profane_word_1", "profane_word_2",

        // Hate/abuse placeholders
        "hate_term_1", "hate_term_2"
    };

    private async Task ValidateNameAsync(
        string value,
        ValidationContext<OnboardUserCommand> validationContext,
        CancellationToken cancellationToken)
    {
        var blocked = _blockedNames;

        if (blocked.Contains(value.Trim().ToLower()))
        {
            validationContext.AddFailure("Name", $"The name '{value}' is not allowed.");
        }
    }
}