using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace Nullbox.Security.Domain.Users;

public enum UserStatus
{
    New,
    Active,
    Suspended,
    Deleted,
    Archived
}