using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Events.Mailboxes;
using Nullbox.Fabric.Domain.ValueObjects;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Mailboxes;

public partial class Mailbox
{
    public Mailbox(Guid accountId, string routingKey, string domain, string name, bool autoCreateAlias, string emailAddress)
    {
        AccountId = accountId;
        RoutingKey = routingKey;
        Domain = domain;
        Name = name;
        AutoCreateAlias = autoCreateAlias;

        EmailAddress = emailAddress;

        DomainEvents.Add(new MailboxCreatedDomainEvent(
            mailbox: this));
    }

    public void Update(string name, bool autoCreateAlias, string emailAddress)
    {
        Name = name;

        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
        DomainEvents.Add(new MailboxUpdatedDomainEvent(
            mailbox: this));
    }
}