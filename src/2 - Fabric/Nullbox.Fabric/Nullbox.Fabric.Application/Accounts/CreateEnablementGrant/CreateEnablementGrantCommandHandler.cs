using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Products;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Accounts.CreateEnablementGrant;

public class CreateEnablementGrantCommandHandler : IRequestHandler<CreateEnablementGrantCommand>
{
    private readonly IEnablementGrantRepository _enablementGrantRepository;
    private readonly IProductDefinitionRepository _productDefinitionRepository;

    public CreateEnablementGrantCommandHandler(
        IEnablementGrantRepository enablementGrantRepository,
        IProductDefinitionRepository productDefinitionRepository)
    {
        _enablementGrantRepository = enablementGrantRepository;
        _productDefinitionRepository = productDefinitionRepository;
    }

    [IntentIgnore]
    public async Task Handle(CreateEnablementGrantCommand request, CancellationToken cancellationToken)
    {
        var productDefinition = await _productDefinitionRepository.FindAsync(x => x.Id == "BETA-001", cancellationToken);

        if (productDefinition == null)
        {
            throw new InvalidOperationException("Product definition not found.");
        }

        var enablementGrant = new EnablementGrant(
            accountId: request.AccountId,
            productKey: productDefinition.Id,
            startsAt: DateTimeOffset.UtcNow,
            endsAt: default,
            deltaMaxMailboxes: productDefinition.MaxMailboxes,
            deltaMaxAliases: productDefinition.MaxAliases,
            deltaMaxAliasesPerMailbox: productDefinition.MaxAliasesPerMailbox,
            deltaMaxBandwidthBytesPerPeriod: productDefinition.MaxBandwidthBytesPerPeriod,
            flags: productDefinition.Flags);

        _enablementGrantRepository.Add(enablementGrant);
    }
}