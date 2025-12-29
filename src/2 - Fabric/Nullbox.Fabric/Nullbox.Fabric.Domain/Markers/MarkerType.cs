using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Markers;

public enum MarkerType
{
    Alias,
    Mailbox,
    Account,
    System
}