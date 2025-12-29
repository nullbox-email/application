using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class AccountRole
{
    public AccountRole(string name, IEnumerable<string> scopes)
    {
        Name = name;
        _scopes = new List<string>(scopes);
    }
}