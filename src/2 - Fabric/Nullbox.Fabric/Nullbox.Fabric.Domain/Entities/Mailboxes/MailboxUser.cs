using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Mailboxes;

public partial class MailboxUser
{
    public MailboxUser(Guid userProfileId, Guid roleId)
    {
        UserProfileId = userProfileId;
        RoleId = roleId;
    }
}