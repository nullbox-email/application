using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Nullbox.Security.Users.Eventing.Messages.Users;

public record ExternalUserCreatedV1Event
{
    public ExternalUserCreatedV1Event()
    {
        Id = null!;
        Context = null!;
    }

    public string Id { get; init; }
    public string Context { get; init; }
    public Guid UserId { get; init; }
}