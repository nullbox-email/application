using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Aliases;

public interface IAliasLearningModeScheduleRepository : IEFRepository<AliasLearningModeSchedule, AliasLearningModeSchedule>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<AliasLearningModeSchedule?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AliasLearningModeSchedule?> FindByIdAsync(Guid id, Func<IQueryable<AliasLearningModeSchedule>, IQueryable<AliasLearningModeSchedule>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<AliasLearningModeSchedule>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}