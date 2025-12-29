using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Events.Aliases;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class Alias
{
    public Alias(Guid mailboxId, Guid accountId, string name, string localPart)
    {
        MailboxId = mailboxId;
        AccountId = accountId;
        Name = name;
        LocalPart = localPart;
        DomainEvents.Add(new AliasCreatedDomainEvent(
            alias: this));
    }

    public void Update(string name, bool isEnabled, bool directPassthrough, bool learningMode)
    {
        Name = name;
        IsEnabled = isEnabled;
        DirectPassthrough = directPassthrough;
        LearningMode = learningMode;
        DomainEvents.Add(new AliasUpdatedDomainEvent(
            alias: this));
    }
}