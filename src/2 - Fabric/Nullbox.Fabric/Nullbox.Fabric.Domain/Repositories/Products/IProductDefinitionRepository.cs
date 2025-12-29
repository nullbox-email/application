using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Products;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Products;

public interface IProductDefinitionRepository : IEFRepository<ProductDefinition, ProductDefinition>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<ProductDefinition?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ProductDefinition?> FindByIdAsync(string id, Func<IQueryable<ProductDefinition>, IQueryable<ProductDefinition>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<ProductDefinition>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}