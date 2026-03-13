using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.DownloadQuarantinedEmlFile;

public class DownloadQuarantinedEmlFileQueryValidator : AbstractValidator<DownloadQuarantinedEmlFileQuery>
{
    public DownloadQuarantinedEmlFileQueryValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.MailboxId)
            .NotNull();

        RuleFor(v => v.AliasId)
            .NotNull();

        RuleFor(v => v.MessageId)
            .NotNull();
    }
}