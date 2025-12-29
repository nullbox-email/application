using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Mailboxes;

public partial class MailboxMap
{
    public MailboxMap(string id, Guid mailboxId, Guid accountId, bool autoCreateAlias, string emailAddress)
    {
        Id = id;
        MailboxId = mailboxId;
        AccountId = accountId;
        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
    }

    public void Update(bool autoCreateAlias, string emailAddress)
    {
        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
    }
}