using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Repositories;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Infrastructure.Persistence;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Repositories.Accounts;

public class EffectiveEnablementRepository : RepositoryBase<EffectiveEnablement, EffectiveEnablement, ApplicationDbContext>, IEffectiveEnablementRepository
{
    public EffectiveEnablementRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
    }

    public async Task<EffectiveEnablement?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<EffectiveEnablement?> FindByIdAsync(
        Guid id,
        Func<IQueryable<EffectiveEnablement>, IQueryable<EffectiveEnablement>> queryOptions,
        CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
    }

    public async Task<List<EffectiveEnablement>> FindByIdsAsync(
        Guid[] ids,
        CancellationToken cancellationToken = default)
    {
        // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
        var idList = ids.ToList();
        return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
    }
}