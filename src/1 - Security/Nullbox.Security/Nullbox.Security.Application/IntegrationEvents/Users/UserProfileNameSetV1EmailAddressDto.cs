using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Security.Users.Eventing.Messages.Users;

public class UserProfileNameSetV1EmailAddressDto
{
    public UserProfileNameSetV1EmailAddressDto()
    {
    }

    public string Value { get; set; }
    public string NormalizedValue { get; set; }

    public static UserProfileNameSetV1EmailAddressDto Create(string value, string normalizedValue)
    {
        return new UserProfileNameSetV1EmailAddressDto
        {
            Value = value,
            NormalizedValue = normalizedValue
        };
    }
}