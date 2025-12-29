using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Aliases;

public interface IAliasMapRepository : IEFRepository<AliasMap, AliasMap>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<AliasMap?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AliasMap?> FindByIdAsync(string id, Func<IQueryable<AliasMap>, IQueryable<AliasMap>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<AliasMap>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}