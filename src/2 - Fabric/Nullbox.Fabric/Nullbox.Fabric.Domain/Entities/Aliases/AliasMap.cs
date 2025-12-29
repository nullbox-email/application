using System.Xml.Linq;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Events.Mailboxes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class AliasMap
{
    public AliasMap(string id,
            Guid mailboxId,
            Guid accountId,
            Guid aliasId,
            bool isEnabled,
            bool directPassthrough,
            bool learningMode,
            bool autoCreateAlias,
            string emailAddress)
    {
        Id = id;
        MailboxId = mailboxId;
        AccountId = accountId;
        AliasId = aliasId;
        IsEnabled = isEnabled;
        DirectPassthrough = directPassthrough;
        LearningMode = learningMode;
        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
    }

    public void UpdateAlias(bool isEnabled, bool directPassthrough, bool learningMode)
    {
        IsEnabled = isEnabled;
        DirectPassthrough = directPassthrough;
        LearningMode = learningMode;
    }

    public void UpdateMailbox(bool autoCreateAlias, string emailAddress)
    {
        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
    }
}