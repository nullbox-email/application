using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Accounts;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Accounts;

public interface IEffectiveEnablementRepository : IEFRepository<EffectiveEnablement, EffectiveEnablement>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<EffectiveEnablement?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EffectiveEnablement?> FindByIdAsync(Guid id, Func<IQueryable<EffectiveEnablement>, IQueryable<EffectiveEnablement>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<EffectiveEnablement>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}