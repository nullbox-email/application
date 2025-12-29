using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Aliases;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Aliases;

public interface IAliasSenderDecisionRepository : IEFRepository<AliasSenderDecision, AliasSenderDecision>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<AliasSenderDecision?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AliasSenderDecision?> FindByIdAsync(string id, Func<IQueryable<AliasSenderDecision>, IQueryable<AliasSenderDecision>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<AliasSenderDecision>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}