using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Events.Accounts;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class Account
{
    public Account(Guid userProfileId, string name, string emailAddress, Guid adminRoleId)
    {
        AccountId = Id;

        Name = name;

        _users.Add(new AccountUser(userProfileId, emailAddress, adminRoleId));

        DomainEvents.Add(new AccountCreatedDomainEvent(
            account: this));
    }

    public void AddUser(Guid userProfileId, string emailAddress, Guid roleId)
    {
        if (userProfileId == Guid.Empty)
        {
            throw new ArgumentException("UserProfileId cannot be empty.", nameof(userProfileId));
        }

        if (_users.Any(u => u.UserProfileId == userProfileId))
        {
            throw new ConflictException($"User with UserProfileId {userProfileId} is already associated with this account.");
        }

        _users.Add(new AccountUser(userProfileId, emailAddress, roleId));
    }

    public void RemoveUser(Guid userProfileId)
    {
        if (!_users.Any(u => u.UserProfileId == userProfileId))
        {
            throw new NotFoundException("No user with the specified UserProfileId is associated with this account.");
        }

        _users.RemoveAll(u => u.UserProfileId == userProfileId);
    }
}