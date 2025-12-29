using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Deliveries;

public enum ProviderStatus
{
    NotAttempted,
    Accepted,
    Failed
}