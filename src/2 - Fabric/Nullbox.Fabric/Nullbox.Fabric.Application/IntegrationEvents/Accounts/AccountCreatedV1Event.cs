using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;

public record AccountCreatedV1Event
{
    public AccountCreatedV1Event()
    {
        Name = null!;
        Users = null!;
    }

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public List<AccountCreatedV1UsersDto> Users { get; init; }
}