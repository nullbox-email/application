using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.ValidatorProvider", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Validation;

public class ValidatorProvider : IValidatorProvider
{
    private readonly IServiceProvider _serviceProvider;

    public ValidatorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IValidator<T> GetValidator<T>()
    {
        return _serviceProvider.GetService<IValidator<T>>()!;
    }
}