using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Deliveries;
using Nullbox.Fabric.Domain.ValueObjects;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Activities;

public partial class RecentDeliveryAction : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected RecentDeliveryAction()
    {
        Id = null!;
        SortKey = null!;
        SenderDisplay = null!;
        SenderDomain = null!;
        RecipientDisplay = null!;
        RecipientDomain = null!;
        Subject = null!;
        SubjectHash = null!;
    }
    public string Id { get; private set; }

    public Guid PartitionKey { get; private set; }

    public Guid DeliveryActionId { get; private set; }

    public DateTimeOffset ReceivedAt { get; private set; }

    public string SortKey { get; private set; }

    public string SenderDisplay { get; private set; }

    public string SenderDomain { get; private set; }

    public string RecipientDisplay { get; private set; }

    public string RecipientDomain { get; private set; }

    public string Subject { get; private set; }

    public string SubjectHash { get; private set; }

    public bool HasAttachments { get; private set; }

    public int AttachmentsCount { get; private set; }

    public long Size { get; private set; }

    public DeliveryDecision DeliveryDecision { get; private set; }

    public DropReasonValueObject? DropReason { get; private set; }

    public QuarantineReasonValueObject? QuarantineReason { get; private set; }

    public string? ForwardTo { get; private set; }

    public string? ForwardFrom { get; private set; }

    public string? Provider { get; private set; }

    public ProviderStatus? ProviderStatus { get; private set; }

    public string? ProviderMessageId { get; private set; }

    public string? ProviderError { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}