using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Domain.Repositories.Products;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Products.InitializeProductCatalog;

public class InitializeProductCatalogHandler : IRequestHandler<InitializeProductCatalog>
{
    private readonly IProductDefinitionRepository _productDefinitionRepository;

    private const long OneKilobyteInBytes = 1024;
    private const long OneMegabyteInBytes = 1024 * OneKilobyteInBytes;
    private const long OneGigabyteInBytes = 1024 * OneMegabyteInBytes;

    public InitializeProductCatalogHandler(
        IProductDefinitionRepository productDefinitionRepository)
    {
        _productDefinitionRepository = productDefinitionRepository;
    }

    public async Task Handle(InitializeProductCatalog request, CancellationToken cancellationToken)
    {
        await CreateOrUpdateProductAsync(
            id: "BETA-001",
            name: "Beta License",
            isActive: true,
            maxMailboxes: 5,
            maxAliases: 50,
            maxAliasesPerMailbox: null,
            maxBandwidthBytesPerPeriod: 1 * OneGigabyteInBytes,
            flags: new Dictionary<string, string>
            {
                { "IsBeta", "true" }
            },
            cancellationToken);

        await CreateOrUpdateProductAsync(
            id: "DEVELOPER-001",
            name: "Developer",
            isActive: true,
            maxMailboxes: null,
            maxAliases: null,
            maxAliasesPerMailbox: null,
            maxBandwidthBytesPerPeriod: 100 * OneGigabyteInBytes,
            flags: new Dictionary<string, string>
            {
                { "IsBeta", "true" }
            },
            cancellationToken);

        await CreateOrUpdateProductAsync(
            id: "FREE",
            name: "Free License",
            isActive: true,
            maxMailboxes: 1,
            maxAliases: 10,
            maxAliasesPerMailbox: null,
            maxBandwidthBytesPerPeriod: 100 * OneMegabyteInBytes,
            flags: new Dictionary<string, string>
            {
            },
            cancellationToken);

        await CreateOrUpdateProductAsync(
            id: "PLUS",
            name: "Plus License",
            isActive: true,
            maxMailboxes: 5,
            maxAliases: 50,
            maxAliasesPerMailbox: null,
            maxBandwidthBytesPerPeriod: 1 * OneGigabyteInBytes,
            flags: new Dictionary<string, string>
            {
            },
            cancellationToken);

        await CreateOrUpdateProductAsync(
            id: "PRO",
            name: "Pro License",
            isActive: true,
            maxMailboxes: null,
            maxAliases: null,
            maxAliasesPerMailbox: null,
            maxBandwidthBytesPerPeriod: 100 * OneGigabyteInBytes,
            flags: new Dictionary<string, string>
            {
            },
            cancellationToken);
    }

    private async Task CreateOrUpdateProductAsync(
        string id,
        string name,
        bool isActive,
        int? maxMailboxes,
        int? maxAliases,
        int? maxAliasesPerMailbox,
        long? maxBandwidthBytesPerPeriod,
        Dictionary<string, string> flags,
        CancellationToken cancellationToken)
    {
        var existingProductDefinition = await _productDefinitionRepository.FindAsync(x => x.Id == id, cancellationToken);

        if (existingProductDefinition is null)
        {
            var newProduct = new Domain.Entities.Products.ProductDefinition(
                id: id,
                name: name,
                isActive: isActive,
                maxMailboxes: maxMailboxes,
                maxAliases: maxAliases,
                maxAliasesPerMailbox: maxAliasesPerMailbox,
                maxBandwidthBytesPerPeriod: maxBandwidthBytesPerPeriod,
                flags: flags);

            _productDefinitionRepository.Add(newProduct);
        }
        else
        {
            existingProductDefinition.Update(
                name: name,
                isActive: isActive,
                maxMailboxes: maxMailboxes,
                maxAliases: maxAliases,
                maxAliasesPerMailbox: maxAliasesPerMailbox,
                maxBandwidthBytesPerPeriod: maxBandwidthBytesPerPeriod,
                flags: flags);

            _productDefinitionRepository.Update(existingProductDefinition);
        }
    }
}