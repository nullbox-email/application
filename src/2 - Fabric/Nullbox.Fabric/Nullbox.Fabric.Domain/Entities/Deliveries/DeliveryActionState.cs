using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Deliveries;
using Nullbox.Fabric.Domain.ValueObjects;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Deliveries;

public partial class DeliveryAction : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected DeliveryAction()
    {
        PartitionKey = null!;
        Source = null!;
        SenderDisplay = null!;
        SenderDomain = null!;
        RecipientDisplay = null!;
        RecipientDomain = null!;
        Subject = null!;
        SubjectHash = null!;
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public string PartitionKey { get; private set; }

    public Guid? AliasId { get; private set; }

    public Guid? MailboxId { get; private set; }

    public Guid? AccountId { get; private set; }

    public string Source { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ReceivedAt { get; private set; }

    public string? Alias { get; private set; }

    public string? RoutingKey { get; private set; }

    public string? Domain { get; private set; }

    public string SenderDisplay { get; private set; }

    public string SenderDomain { get; private set; }

    public string RecipientDisplay { get; private set; }

    public string RecipientDomain { get; private set; }

    public string? MessageId { get; private set; }

    public string Subject { get; private set; }

    public string SubjectHash { get; private set; }

    public bool HasAttachments { get; private set; } = false;

    public int AttachmentsCount { get; private set; } = 0;

    public long Size { get; private set; } = 0;

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

    public string? DedupKey { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}