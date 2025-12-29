using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Entities.Mailboxes;

[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Events.Mailboxes;

public class MailboxCreatedDomainEvent : DomainEvent
{
    public MailboxCreatedDomainEvent(Mailbox mailbox)
    {
        Mailbox = mailbox;
    }

    public Mailbox Mailbox { get; }
}