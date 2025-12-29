using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.GetUserMailboxByRoutingKeyAndDomain;

public class GetUserMailboxByRoutingKeyAndDomainQueryValidator : AbstractValidator<GetUserMailboxByRoutingKeyAndDomainQuery>
{
    public GetUserMailboxByRoutingKeyAndDomainQueryValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        RuleFor(v => v.Id)
            .NotNull();
    }
}