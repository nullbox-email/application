using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Markers;
using Nullbox.Fabric.Domain.Repositories;
using Nullbox.Fabric.Domain.Repositories.Markers;
using Nullbox.Fabric.Infrastructure.Persistence;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Repositories.Markers;

public class AppliedMarkerRepository : RepositoryBase<AppliedMarker, AppliedMarker, ApplicationDbContext>, IAppliedMarkerRepository
{
    public AppliedMarkerRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
        string id,
        CancellationToken cancellationToken = default)
    {
        return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
    }

    public async Task<AppliedMarker?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<AppliedMarker?> FindByIdAsync(
        string id,
        Func<IQueryable<AppliedMarker>, IQueryable<AppliedMarker>> queryOptions,
        CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
    }

    public async Task<List<AppliedMarker>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
    {
        // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
        var idList = ids.ToList();
        return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
    }
}