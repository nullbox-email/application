using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Users;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Nullbox.Security.Users.Eventing.Messages.Users;

public record UserProfileDeletedV1Event
{
    public UserProfileDeletedV1Event()
    {
        Name = null!;
        EmailAddress = null!;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public UserProfileDeletedV1EmailAddressDto EmailAddress { get; init; }
    public UserStatus Status { get; init; }
}