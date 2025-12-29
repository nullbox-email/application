using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Accounts;

public interface IAccountRoleRepository : IEFRepository<AccountRole, AccountRole>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<AccountRole?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AccountRole?> FindByIdAsync(Guid id, Func<IQueryable<AccountRole>, IQueryable<AccountRole>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<AccountRole>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}