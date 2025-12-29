using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Rollups;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Rollups;

public interface ITopAliasRepository : IEFRepository<TopAlias, TopAlias>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<TopAlias?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TopAlias?> FindByIdAsync(string id, Func<IQueryable<TopAlias>, IQueryable<TopAlias>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TopAlias>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}