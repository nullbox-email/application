using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Aliases;
using Nullbox.Fabric.Domain.Repositories;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Infrastructure.Persistence;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Nullbox.Fabric.Infrastructure.Repositories.Aliases;

public class AliasRepository : RepositoryBase<Alias, Alias, ApplicationDbContext>, IAliasRepository
{
    public AliasRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
    }

    public async Task<Alias?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Alias?> FindByIdAsync(
        Guid id,
        Func<IQueryable<Alias>, IQueryable<Alias>> queryOptions,
        CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
    }

    public async Task<List<Alias>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
        var idList = ids.ToList();
        return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
    }
}