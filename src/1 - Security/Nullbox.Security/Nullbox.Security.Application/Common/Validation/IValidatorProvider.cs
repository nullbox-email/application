using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.ValidatorProviderInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Validation;

public interface IValidatorProvider
{
    IValidator<T> GetValidator<T>();
}