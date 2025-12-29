using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Security.Users.Eventing.Messages.Users;

public class UserProfileDeletedV1EmailAddressDto
{
    public UserProfileDeletedV1EmailAddressDto()
    {
    }

    public string Value { get; set; }
    public string NormalizedValue { get; set; }

    public static UserProfileDeletedV1EmailAddressDto Create(string value, string normalizedValue)
    {
        return new UserProfileDeletedV1EmailAddressDto
        {
            Value = value,
            NormalizedValue = normalizedValue
        };
    }
}