using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class AccountUser
{
    public AccountUser(Guid userProfileId, string emailAddress, Guid roleId)
    {
        UserProfileId = userProfileId;
        EmailAddress = emailAddress;
        RoleId = roleId;
    }
}