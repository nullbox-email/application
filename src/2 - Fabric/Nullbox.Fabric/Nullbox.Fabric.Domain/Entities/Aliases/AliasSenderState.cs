using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class AliasSender : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected AliasSender()
    {
        Id = null!;
        Email = null!;
        Domain = null!;
    }

    public string Id { get; private set; }

    public Guid AliasId { get; private set; }

    public string Email { get; private set; }

    public string Domain { get; private set; }

    public DateTimeOffset FirstSeenAt { get; private set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset LastSeenAt { get; private set; } = DateTimeOffset.UtcNow;

    public int SeenCount { get; private set; } = 1;

    public DeliveryDecision? LastDecision { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}