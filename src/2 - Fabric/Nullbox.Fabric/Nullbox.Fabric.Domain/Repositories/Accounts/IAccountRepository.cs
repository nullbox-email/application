using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Accounts;

public interface IAccountRepository : IEFRepository<Account, Account>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<Account?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Account?> FindByIdAsync(Guid id, Func<IQueryable<Account>, IQueryable<Account>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<Account>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}