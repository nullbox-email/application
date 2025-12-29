using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Nullbox.Security.Application.OIDC.GetJWKS;

public class GetJWKSQueryValidator : AbstractValidator<GetJWKSQuery>
{
    public GetJWKSQueryValidator()
    {
        ConfigureValidationRules();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Depends on user code")]
    private void ConfigureValidationRules()
    {
        // Implement custom validation logic here if required
    }
}