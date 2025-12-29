using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Accounts;

public interface IEnablementGrantRepository : IEFRepository<EnablementGrant, EnablementGrant>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<EnablementGrant?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EnablementGrant?> FindByIdAsync(Guid id, Func<IQueryable<EnablementGrant>, IQueryable<EnablementGrant>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<EnablementGrant>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}