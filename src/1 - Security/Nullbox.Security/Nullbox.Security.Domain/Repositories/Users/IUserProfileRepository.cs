using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Entities.Users;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Security.Domain.Repositories.Users;

public interface IUserProfileRepository : IEFRepository<UserProfile, UserProfile>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<UserProfile?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserProfile?> FindByIdAsync(Guid id, Func<IQueryable<UserProfile>, IQueryable<UserProfile>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<UserProfile>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}