using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Nullbox.Security.Users.Eventing.Messages.Users;

public class UserProfileSuspendedV1EmailAddressDto
{
    public UserProfileSuspendedV1EmailAddressDto()
    {
    }

    public string Value { get; set; }
    public string NormalizedValue { get; set; }

    public static UserProfileSuspendedV1EmailAddressDto Create(string value, string normalizedValue)
    {
        return new UserProfileSuspendedV1EmailAddressDto
        {
            Value = value,
            NormalizedValue = normalizedValue
        };
    }
}