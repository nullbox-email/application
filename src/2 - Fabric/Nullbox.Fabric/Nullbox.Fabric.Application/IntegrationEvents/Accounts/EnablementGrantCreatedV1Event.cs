using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Accounts;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Nullbox.Fabric.Accounts.Eventing.Messages.Accounts;

public record EnablementGrantCreatedV1Event
{
    public EnablementGrantCreatedV1Event()
    {
        Flags = null!;
    }

    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public EnablementKind Kind { get; init; }
    public string? ProductKey { get; init; }
    public int Priority { get; init; }
    public DateTimeOffset? StartsAt { get; init; }
    public DateTimeOffset? EndsAt { get; init; }
    public int? DeltaMaxMailboxes { get; init; }
    public int? DeltaMaxAliases { get; init; }
    public int? DeltaMaxAliasesPerMailbox { get; init; }
    public long? DeltaMaxBandwidthBytesPerPeriod { get; init; }
    public Dictionary<string, string> Flags { get; init; }
    public EnablementSource Source { get; init; }
    public string? Reason { get; init; }
    public bool IsRevoked { get; init; }
}