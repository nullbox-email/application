using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Entities.Users;
using Nullbox.Security.Domain.Repositories;
using Nullbox.Security.Domain.Repositories.Users;
using Nullbox.Security.Infrastructure.Persistence;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Nullbox.Security.Infrastructure.Repositories.Users;

public class ExternalUserRepository : RepositoryBase<ExternalUser, ExternalUser, ApplicationDbContext>, IExternalUserRepository
{
    public ExternalUserRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
        string id,
        CancellationToken cancellationToken = default)
    {
        return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
    }

    public async Task<ExternalUser?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ExternalUser?> FindByIdAsync(
        string id,
        Func<IQueryable<ExternalUser>, IQueryable<ExternalUser>> queryOptions,
        CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
    }

    public async Task<List<ExternalUser>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default)
    {
        // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
        var idList = ids.ToList();
        return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
    }
}