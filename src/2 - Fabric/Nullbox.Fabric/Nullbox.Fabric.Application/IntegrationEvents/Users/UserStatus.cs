using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventEnum", Version = "1.0")]

namespace Nullbox.Fabric.Application.IntegrationEvents.Users;

public enum UserStatus
{
    New,
    Active,
    Suspended,
    Deleted,
    Archived
}