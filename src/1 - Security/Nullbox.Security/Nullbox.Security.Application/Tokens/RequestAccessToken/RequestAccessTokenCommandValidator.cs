using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Security.Application.Tokens.RequestAccessToken;

public class RequestAccessTokenCommandValidator : AbstractValidator<RequestAccessTokenCommand>
{
    public RequestAccessTokenCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Scopes)
            .NotNull();
    }
}