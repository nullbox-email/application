using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Mailboxes;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Repositories.Mailboxes;

public interface IMailboxRoutingKeyMapRepository : IEFRepository<MailboxRoutingKeyMap, MailboxRoutingKeyMap>
{
    Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
    Task<MailboxRoutingKeyMap?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<MailboxRoutingKeyMap?> FindByIdAsync(string id, Func<IQueryable<MailboxRoutingKeyMap>, IQueryable<MailboxRoutingKeyMap>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<MailboxRoutingKeyMap>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
}