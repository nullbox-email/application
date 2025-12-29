using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Mailboxes;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Mailboxes;

public interface IMailboxMapRepository : IEFRepository<MailboxMap, MailboxMap>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<MailboxMap?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<MailboxMap?> FindByIdAsync(string id, Func<IQueryable<MailboxMap>, IQueryable<MailboxMap>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<MailboxMap>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}