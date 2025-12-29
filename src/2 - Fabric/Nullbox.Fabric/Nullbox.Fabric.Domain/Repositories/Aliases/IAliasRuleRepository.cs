using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Aliases;

public interface IAliasRuleRepository : IEFRepository<AliasRule, AliasRule>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<AliasRule?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AliasRule?> FindByIdAsync(string id, Func<IQueryable<AliasRule>, IQueryable<AliasRule>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<AliasRule>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}