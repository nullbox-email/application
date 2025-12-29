using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Deliveries.ProcessEmailComplete;

public class ProcessEmailCompleteCommand : IRequest, ICommand
{
    public ProcessEmailCompleteCommand(Guid deliveryActionId,
        string partitionKey,
        DateTimeOffset completedAtUtc,
        string outcome,
        string? reason,
        string? error,
        string? alias,
        string? routingKey,
        string? domain,
        string? recipient,
        string? forwardTo,
        string? provider,
        string? providerMessageId,
        string? source)
    {
        DeliveryActionId = deliveryActionId;
        PartitionKey = partitionKey;
        CompletedAtUtc = completedAtUtc;
        Outcome = outcome;
        Reason = reason;
        Error = error;
        Alias = alias;
        RoutingKey = routingKey;
        Domain = domain;
        Recipient = recipient;
        ForwardTo = forwardTo;
        Provider = provider;
        ProviderMessageId = providerMessageId;
        Source = source;
    }

    public Guid DeliveryActionId { get; set; }
    public string PartitionKey { get; set; }
    public DateTimeOffset CompletedAtUtc { get; set; }
    public string Outcome { get; set; }
    public string? Reason { get; set; }
    public string? Error { get; set; }
    public string? Alias { get; set; }
    public string? RoutingKey { get; set; }
    public string? Domain { get; set; }
    public string? Recipient { get; set; }
    public string? ForwardTo { get; set; }
    public string? Provider { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? Source { get; set; }
}