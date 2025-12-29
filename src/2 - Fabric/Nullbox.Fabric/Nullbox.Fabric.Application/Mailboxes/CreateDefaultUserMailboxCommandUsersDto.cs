using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes;

public class CreateDefaultUserMailboxCommandUsersDto
{
    public CreateDefaultUserMailboxCommandUsersDto()
    {
        EmailAddress = null!;
    }

    public Guid AccountId { get; set; }
    public Guid UserProfileId { get; set; }
    public string EmailAddress { get; set; }
    public Guid RoleId { get; set; }

    public static CreateDefaultUserMailboxCommandUsersDto Create(
            Guid accountId,
            Guid userProfileId,
            string emailAddress,
            Guid roleId)
    {
        return new CreateDefaultUserMailboxCommandUsersDto
        {
            AccountId = accountId,
            UserProfileId = userProfileId,
            EmailAddress = emailAddress,
            RoleId = roleId
        };
    }
}