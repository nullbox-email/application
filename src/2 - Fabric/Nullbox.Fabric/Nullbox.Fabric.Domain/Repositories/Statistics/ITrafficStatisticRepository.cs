using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Statistics;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Statistics;

public interface ITrafficStatisticRepository : IEFRepository<TrafficStatistic, TrafficStatistic>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<TrafficStatistic?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TrafficStatistic?> FindByIdAsync(string id, Func<IQueryable<TrafficStatistic>, IQueryable<TrafficStatistic>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TrafficStatistic>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}