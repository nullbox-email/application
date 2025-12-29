using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Accounts;

public interface IAccountUserMapRepository : IEFRepository<AccountUserMap, AccountUserMap>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<AccountUserMap?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AccountUserMap?> FindByIdAsync(Guid id, Func<IQueryable<AccountUserMap>, IQueryable<AccountUserMap>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<AccountUserMap>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}