using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class AccountRole
{
    public AccountRole(Guid id, Guid accountId, string name, IEnumerable<string> scopes)
    {
        Id = id;
        AccountId = accountId;
        Name = name;
        _scopes = new List<string>(scopes);
    }
}