using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Deliveries.QuarantineEmail;

public class QuarantineEmailCommand : IRequest, ICommand
{
    public QuarantineEmailCommand(Guid deliveryActionId,
        string partitionKey,
        QuarantineReason reason,
        string message,
        Stream content,
        string? filename,
        string? contentType,
        long? contentLength)
    {
        DeliveryActionId = deliveryActionId;
        PartitionKey = partitionKey;
        Reason = reason;
        Message = message;
        Content = content;
        Filename = filename;
        ContentType = contentType;
        ContentLength = contentLength;
    }

    public Guid DeliveryActionId { get; set; }
    public string PartitionKey { get; set; }
    public QuarantineReason Reason { get; set; }
    public string Message { get; set; }
    public Stream Content { get; set; }
    public string? Filename { get; set; }
    public string? ContentType { get; set; }
    public long? ContentLength { get; set; }
}