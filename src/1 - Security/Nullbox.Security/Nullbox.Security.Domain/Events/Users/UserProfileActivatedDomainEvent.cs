using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Common;
using Nullbox.Security.Domain.Entities.Users;

[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Nullbox.Security.Domain.Events.Users;

public class UserProfileActivatedDomainEvent : DomainEvent
{
    public UserProfileActivatedDomainEvent(UserProfile userProfile)
    {
        UserProfile = userProfile;
    }

    public UserProfile UserProfile { get; }
}