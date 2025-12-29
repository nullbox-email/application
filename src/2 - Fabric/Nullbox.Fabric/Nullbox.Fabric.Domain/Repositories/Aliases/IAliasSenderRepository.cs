using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Aliases;

public interface IAliasSenderRepository : IEFRepository<AliasSender, AliasSender>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<AliasSender?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AliasSender?> FindByIdAsync(string id, Func<IQueryable<AliasSender>, IQueryable<AliasSender>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<AliasSender>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}