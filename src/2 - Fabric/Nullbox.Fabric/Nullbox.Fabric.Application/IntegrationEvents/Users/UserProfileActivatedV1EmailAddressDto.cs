using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Security.Users.Eventing.Messages.Users;

public class UserProfileActivatedV1EmailAddressDto
{
    public UserProfileActivatedV1EmailAddressDto()
    {
    }

    public string Value { get; set; }
    public string NormalizedValue { get; set; }

    public static UserProfileActivatedV1EmailAddressDto Create(string value, string normalizedValue)
    {
        return new UserProfileActivatedV1EmailAddressDto
        {
            Value = value,
            NormalizedValue = normalizedValue
        };
    }
}