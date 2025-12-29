using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Activities;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Activities;

public interface IRecentDeliveryActionRepository : IEFRepository<RecentDeliveryAction, RecentDeliveryAction>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<RecentDeliveryAction?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<RecentDeliveryAction?> FindByIdAsync(string id, Func<IQueryable<RecentDeliveryAction>, IQueryable<RecentDeliveryAction>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<RecentDeliveryAction>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}