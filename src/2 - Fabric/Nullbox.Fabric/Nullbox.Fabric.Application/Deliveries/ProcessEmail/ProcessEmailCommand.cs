using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Deliveries.ProcessEmail;

public class ProcessEmailCommand : IRequest<DeliveryDecisionDto>, ICommand
{
    public ProcessEmailCommand(string alias,
            string routingKey,
            string domain,
            string sender,
            string senderDomain,
            string recipient,
            string recipientDomain,
            string? messageId,
            string subject,
            string subjectHash,
            bool hasAttachments,
            int attachmentsCount,
            long size,
            DateTimeOffset receivedAtUtc,
            string source)
    {
        Alias = alias;
        RoutingKey = routingKey;
        Domain = domain;
        Sender = sender;
        SenderDomain = senderDomain;
        Recipient = recipient;
        RecipientDomain = recipientDomain;
        MessageId = messageId;
        Subject = subject;
        SubjectHash = subjectHash;
        HasAttachments = hasAttachments;
        AttachmentsCount = attachmentsCount;
        Size = size;
        ReceivedAtUtc = receivedAtUtc;
        Source = source;
    }
    public string Alias { get; set; }
    public string RoutingKey { get; set; }
    public string Domain { get; set; }
    public string Sender { get; set; }
    public string SenderDomain { get; set; }
    public string Recipient { get; set; }
    public string RecipientDomain { get; set; }
    public string? MessageId { get; set; }
    public string Subject { get; set; }
    public string SubjectHash { get; set; }
    public bool HasAttachments { get; set; }
    public int AttachmentsCount { get; set; }
    public long Size { get; set; }
    public DateTimeOffset ReceivedAtUtc { get; set; }
    public string Source { get; set; }
}