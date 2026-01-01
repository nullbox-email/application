using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Audit;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Audit;

public interface IAuditLogEntryRepository : IEFRepository<AuditLogEntry, AuditLogEntry>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<AuditLogEntry?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AuditLogEntry?> FindByIdAsync(Guid id, Func<IQueryable<AuditLogEntry>, IQueryable<AuditLogEntry>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<AuditLogEntry>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}