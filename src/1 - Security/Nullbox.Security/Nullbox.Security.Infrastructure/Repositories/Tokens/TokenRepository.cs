using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Entities.Tokens;
using Nullbox.Security.Domain.Repositories;
using Nullbox.Security.Domain.Repositories.Tokens;
using Nullbox.Security.Infrastructure.Persistence;

[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Nullbox.Security.Infrastructure.Repositories.Tokens;

public class TokenRepository : RepositoryBase<Token, Token, ApplicationDbContext>, ITokenRepository
{
    public TokenRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
    }

    public async Task<Token?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Token?> FindByIdAsync(
        Guid id,
        Func<IQueryable<Token>, IQueryable<Token>> queryOptions,
        CancellationToken cancellationToken = default)
    {
        return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
    }

    public async Task<List<Token>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
        var idList = ids.ToList();
        return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
    }
}