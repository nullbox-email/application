using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Entities.Tokens;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Security.Domain.Repositories.Tokens;

public interface ITokenRepository : IEFRepository<Token, Token>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<Token?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Token?> FindByIdAsync(Guid id, Func<IQueryable<Token>, IQueryable<Token>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<Token>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}