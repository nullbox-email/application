using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.ValidationServiceInterface", Version = "1.0")]

namespace Nullbox.Fabric.Application.Common.Validation;

public interface IValidationService
{
    Task Handle<TRequest>(TRequest request, CancellationToken cancellationToken = default);
}