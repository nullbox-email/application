using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Deliveries;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Deliveries;

public interface IDeliveryActionRepository : IEFRepository<DeliveryAction, DeliveryAction>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<DeliveryAction?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DeliveryAction?> FindByIdAsync(Guid id, Func<IQueryable<DeliveryAction>, IQueryable<DeliveryAction>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<DeliveryAction>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}