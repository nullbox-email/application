using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Rollups;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Rollups;

public interface ITopDomainRepository : IEFRepository<TopDomain, TopDomain>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<TopDomain?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TopDomain?> FindByIdAsync(string id, Func<IQueryable<TopDomain>, IQueryable<TopDomain>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TopDomain>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}