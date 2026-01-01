using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Products;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Accounts.CreateEnablementGrant;

public class CreateEnablementGrantCommandHandler : IRequestHandler<CreateEnablementGrantCommand>
{
    private readonly IEnablementGrantRepository _enablementGrantRepository;
    private readonly IProductDefinitionRepository _productDefinitionRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public CreateEnablementGrantCommandHandler(
        IEnablementGrantRepository enablementGrantRepository,
        IProductDefinitionRepository productDefinitionRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _enablementGrantRepository = enablementGrantRepository;
        _productDefinitionRepository = productDefinitionRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    [IntentIgnore]
    public async Task Handle(CreateEnablementGrantCommand request, CancellationToken cancellationToken)
    {
        var productDefinition = await _productDefinitionRepository.FindAsync(x => x.Id == "BETA-001", cancellationToken);

        if (productDefinition == null)
        {
            throw new InvalidOperationException("Product definition not found.");
        }

        using var _ = _partitionKeyScope.Push(productDefinition.Id);

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