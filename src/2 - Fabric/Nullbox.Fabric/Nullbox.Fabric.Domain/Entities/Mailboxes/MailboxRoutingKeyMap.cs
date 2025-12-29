using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Mailboxes;

public partial class MailboxRoutingKeyMap
{
    public MailboxRoutingKeyMap(string id, Guid mailboxId, Guid accountId)
    {
        Id = id;
        MailboxId = mailboxId;
        AccountId = accountId;
    }
}