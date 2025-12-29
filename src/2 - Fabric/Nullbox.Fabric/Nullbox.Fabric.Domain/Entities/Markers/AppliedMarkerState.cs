using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Markers;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Markers;

[IntentIgnore]
public partial class AppliedMarker : IHasDomainEvent
{
    /// <summary>
    /// g:{group}|k:{key}|s:{stage}|da:{deliveryActionId}
    /// </summary>
    public string Id { get; set; }

    public string PartitionKey { get; set; }

    public Guid DeliveryActionId { get; set; }

    public MarkerStage Stage { get; set; }

    public MarkerGroup Group { get; set; }

    public string Key { get; set; }

    public DateTimeOffset AppliedAt { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}