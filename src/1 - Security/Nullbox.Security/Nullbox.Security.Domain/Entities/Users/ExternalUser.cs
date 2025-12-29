using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Events.Users;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Security.Domain.Entities.Users;

public partial class ExternalUser
{
    public ExternalUser(string id, string context, Guid userId)
    {
        Id = id;
        Context = context;
        UserId = userId;

        DomainEvents.Add(new ExternalUserCreatedDomainEvent(
            externalUser: this));
    }
}