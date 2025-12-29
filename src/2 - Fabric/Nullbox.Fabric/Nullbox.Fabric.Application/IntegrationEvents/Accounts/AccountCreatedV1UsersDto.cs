using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;

public class AccountCreatedV1UsersDto
{
    public AccountCreatedV1UsersDto()
    {
    }

    public Guid AccountId { get; set; }
    public Guid UserProfileId { get; set; }
    public string EmailAddress { get; set; }
    public Guid RoleId { get; set; }

    public static AccountCreatedV1UsersDto Create(Guid accountId, Guid userProfileId, string emailAddress, Guid roleId)
    {
        return new AccountCreatedV1UsersDto
        {
            AccountId = accountId,
            UserProfileId = userProfileId,
            EmailAddress = emailAddress,
            RoleId = roleId
        };
    }
}