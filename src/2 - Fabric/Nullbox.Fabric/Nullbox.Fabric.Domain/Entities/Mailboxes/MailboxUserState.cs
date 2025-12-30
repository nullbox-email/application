using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Mailboxes;

public partial class MailboxUser
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected MailboxUser()
    {
    }
    public Guid MailboxId { get; private set; }

    public Guid UserProfileId { get; private set; }

    public Guid RoleId { get; private set; }
}