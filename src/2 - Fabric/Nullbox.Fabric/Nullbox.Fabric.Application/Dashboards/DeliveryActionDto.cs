using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Dashboards;

public class DeliveryActionDto
{
    public DeliveryActionDto()
    {
        Id = null!;
        SenderDisplay = null!;
        SenderDomain = null!;
        Subject = null!;
        MessageOutcome = null!;
    }

    public string Id { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public string SenderDisplay { get; set; }
    public string SenderDomain { get; set; }
    public string Subject { get; set; }
    public string MessageOutcome { get; set; }
    public string? Reason { get; set; }
    public string? ProviderStatus { get; set; }
    public bool? HasAttachments { get; set; }
    public int? AttachmentsCount { get; set; }
    public long? Size { get; set; }

    public static DeliveryActionDto Create(
        string id,
        DateTimeOffset receivedAt,
        string senderDisplay,
        string senderDomain,
        string subject,
        string messageOutcome,
        string? reason,
        string? providerStatus,
        bool? hasAttachments,
        int? attachmentsCount,
        long? size)
    {
        return new DeliveryActionDto
        {
            Id = id,
            ReceivedAt = receivedAt,
            SenderDisplay = senderDisplay,
            SenderDomain = senderDomain,
            Subject = subject,
            MessageOutcome = messageOutcome,
            Reason = reason,
            ProviderStatus = providerStatus,
            HasAttachments = hasAttachments,
            AttachmentsCount = attachmentsCount,
            Size = size
        };
    }
}