using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Mailboxes;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Mailboxes;

public interface IMailboxRepository : IEFRepository<Mailbox, Mailbox>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
    Task<Mailbox?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Mailbox?> FindByIdAsync(Guid id, Func<IQueryable<Mailbox>, IQueryable<Mailbox>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<Mailbox>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
}