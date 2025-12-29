using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Aliases;
using Nullbox.Fabric.Domain.Repositories;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Infrastructure.Persistence;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Repositories.Aliases;

public class AliasLearningModeScheduleRepository : RepositoryBase<AliasLearningModeSchedule, AliasLearningModeSchedule, ApplicationDbContext>, IAliasLearningModeScheduleRepository
{
    public AliasLearningModeScheduleRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
    }

    public async Task<AliasLearningModeSchedule?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<AliasLearningModeSchedule?> FindByIdAsync(
        Guid id,
        Func<IQueryable<AliasLearningModeSchedule>, IQueryable<AliasLearningModeSchedule>> queryOptions,
        CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
    }

    public async Task<List<AliasLearningModeSchedule>> FindByIdsAsync(
        Guid[] ids,
        CancellationToken cancellationToken = default)
    {
        // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
        var idList = ids.ToList();
        return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
    }
}