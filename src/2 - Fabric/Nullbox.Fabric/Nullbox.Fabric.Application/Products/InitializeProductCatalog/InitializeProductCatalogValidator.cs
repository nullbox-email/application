using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Products.InitializeProductCatalog;

public class InitializeProductCatalogValidator : AbstractValidator<InitializeProductCatalog>
{
    public InitializeProductCatalogValidator()
    {
        ConfigureValidationRules();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Depends on user code")]
    private void ConfigureValidationRules()
    {
        // Implement custom validation logic here if required
    }
}