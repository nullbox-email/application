using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Events.Users;
using Nullbox.Security.Domain.Users;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Security.Domain.Entities.Users;

public partial class UserProfile
{
    public UserProfile(string emailAddress)
    {
        EmailAddress = new EmailAddressValueObject(emailAddress, emailAddress.ToLower());

        DomainEvents.Add(new UserProfileCreatedDomainEvent(
            userProfile: this));
    }

    public void SetName(string name)
    {
        Name = name;

        DomainEvents.Add(new UserProfileNameSetDomainEvent(
            userProfile: this));
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
        {
            throw new InvalidOperationException("User is already active.");
        }

        if (Status == UserStatus.Deleted)
        {
            throw new InvalidOperationException("Cannot activate a deleted user.");
        }

        if (Status == UserStatus.Archived)
        {
            throw new InvalidOperationException("Cannot activate an archived user.");
        }

        Status = UserStatus.Active;

        DomainEvents.Add(new UserProfileActivatedDomainEvent(
            userProfile: this));
    }

    public void Suspend()
    {
        if (Status != UserStatus.Active)
        {
            throw new InvalidOperationException("Only active users can be suspended.");
        }

        Status = UserStatus.Suspended;

        DomainEvents.Add(new UserProfileSuspendedDomainEvent(
            userProfile: this));
    }

    public void Delete()
    {
        if (Status == UserStatus.Deleted)
        {
            throw new InvalidOperationException("User is already deleted.");
        }

        if (Status == UserStatus.Archived)
        {
            throw new InvalidOperationException("Cannot delete an archived user.");
        }

        Status = UserStatus.Deleted;

        DomainEvents.Add(new UserProfileDeletedDomainEvent(
            userProfile: this));
    }

    public void Archive()
    {
        if (Status != UserStatus.Deleted)
        {
            throw new InvalidOperationException("Only deleted users can be archived.");
        }

        Status = UserStatus.Archived;

        DomainEvents.Add(new UserProfileArchivedDomainEvent(
            userProfile: this));
    }
}