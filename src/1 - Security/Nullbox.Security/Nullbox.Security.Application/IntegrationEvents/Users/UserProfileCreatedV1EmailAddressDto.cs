using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Security.Users.Eventing.Messages.Users;

public class UserProfileCreatedV1EmailAddressDto
{
    public UserProfileCreatedV1EmailAddressDto()
    {
    }

    public string Value { get; set; }
    public string NormalizedValue { get; set; }

    public static UserProfileCreatedV1EmailAddressDto Create(string value, string normalizedValue)
    {
        return new UserProfileCreatedV1EmailAddressDto
        {
            Value = value,
            NormalizedValue = normalizedValue
        };
    }
}