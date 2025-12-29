using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Markers;

public enum MarkerGroup
{
    Traffic,
    TopAliases,
    TopMailboxes,
    TopTargetDomains,
    TopSenderDomains,
    Activities
}