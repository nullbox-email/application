using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;

public record AccountCreateEnablementV1
{
    public AccountCreateEnablementV1()
    {
        Name = null!;
        Users = null!;
    }

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public List<AccountCreatedV1UsersDto> Users { get; init; }
}