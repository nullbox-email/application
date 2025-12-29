using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class AccountUserMap
{
    public AccountUserMap(Guid id, Guid userId)
    {
        Id = id;
        PartitionKey = userId;
    }
}