using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Markers;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Markers;

public interface IAppliedMarkerRepository : IEFRepository<AppliedMarker, AppliedMarker>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<AppliedMarker?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AppliedMarker?> FindByIdAsync(string id, Func<IQueryable<AppliedMarker>, IQueryable<AppliedMarker>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<AppliedMarker>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}