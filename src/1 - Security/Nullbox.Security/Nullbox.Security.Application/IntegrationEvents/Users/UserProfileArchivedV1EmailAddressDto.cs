using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Security.Users.Eventing.Messages.Users;

public class UserProfileArchivedV1EmailAddressDto
{
    public UserProfileArchivedV1EmailAddressDto()
    {
    }

    public string Value { get; set; }
    public string NormalizedValue { get; set; }

    public static UserProfileArchivedV1EmailAddressDto Create(string value, string normalizedValue)
    {
        return new UserProfileArchivedV1EmailAddressDto
        {
            Value = value,
            NormalizedValue = normalizedValue
        };
    }
}