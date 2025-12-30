using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Security.Application.Users.CloudflareSiteVerify;

public class CloudflareSiteVerifyCommandValidator : AbstractValidator<CloudflareSiteVerifyCommand>
{
    public CloudflareSiteVerifyCommandValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Secret)
            .NotNull();

        RuleFor(v => v.Response)
            .NotNull();
    }
}