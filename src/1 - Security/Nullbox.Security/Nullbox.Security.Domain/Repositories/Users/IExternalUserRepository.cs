using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Entities.Users;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Security.Domain.Repositories.Users;

public interface IExternalUserRepository : IEFRepository<ExternalUser, ExternalUser>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<ExternalUser?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ExternalUser?> FindByIdAsync(string id, Func<IQueryable<ExternalUser>, IQueryable<ExternalUser>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<ExternalUser>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}