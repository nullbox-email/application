using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class AccountUser
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected AccountUser()
    {
        EmailAddress = null!;
    }
    public Guid AccountId { get; private set; }

    public Guid UserProfileId { get; private set; }

    public string EmailAddress { get; private set; }

    public Guid RoleId { get; private set; }
}