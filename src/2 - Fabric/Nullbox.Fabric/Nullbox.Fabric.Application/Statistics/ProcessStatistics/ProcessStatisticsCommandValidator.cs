using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Application.Common.Validation;
using Nullbox.Fabric.Application.Deliveries;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Statistics.ProcessStatistics;

public class ProcessStatisticsCommandValidator : AbstractValidator<ProcessStatisticsCommand>
{
    public ProcessStatisticsCommandValidator(IValidatorProvider provider)
    {
        ConfigureValidationRules(provider);
    }

    private void ConfigureValidationRules(IValidatorProvider provider)
    {
        RuleFor(v => v.PartitionKey)
            .NotNull();

        RuleFor(v => v.Source)
            .NotNull();

        RuleFor(v => v.SenderDisplay)
            .NotNull();

        RuleFor(v => v.SenderDomain)
            .NotNull();

        RuleFor(v => v.RecipientDisplay)
            .NotNull();

        RuleFor(v => v.RecipientDomain)
            .NotNull();

        RuleFor(v => v.Subject)
            .NotNull();

        RuleFor(v => v.SubjectHash)
            .NotNull();

        RuleFor(v => v.DeliveryDecision)
            .NotNull()
            .IsInEnum();

        RuleFor(v => v.DropReason)
            .SetValidator(provider.GetValidator<ProcessStatisticsCommandDropReasonDto>()!);

        RuleFor(v => v.QuarantineReason)
            .SetValidator(provider.GetValidator<ProcessStatisticsCommandQuarantineReasonDto>()!);

        RuleFor(v => v.ProviderStatus)
            .IsInEnum();
    }
}