using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Aliases;

public interface IAliasRepository : IEFRepository<Alias, Alias>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<Alias?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Alias?> FindByIdAsync(Guid id, Func<IQueryable<Alias>, IQueryable<Alias>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<Alias>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}