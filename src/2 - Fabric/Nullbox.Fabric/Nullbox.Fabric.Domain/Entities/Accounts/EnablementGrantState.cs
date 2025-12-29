using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Accounts;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class EnablementGrant : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected EnablementGrant()
    {
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public Guid AccountId { get; private set; }

    public EnablementKind Kind { get; private set; }

    public string? ProductKey { get; private set; }

    public int Priority { get; private set; } = 0;

    public DateTimeOffset? StartsAt { get; private set; }

    public DateTimeOffset? EndsAt { get; private set; }

    public int? DeltaMaxMailboxes { get; private set; }

    public int? DeltaMaxAliases { get; private set; }

    public int? DeltaMaxAliasesPerMailbox { get; private set; }

    public long? DeltaMaxBandwidthBytesPerPeriod { get; private set; }

    public Dictionary<string, string> Flags { get; private set; } = [];

    public EnablementSource Source { get; private set; } = EnablementSource.System;

    public string? Reason { get; private set; }

    public bool IsRevoked { get; private set; } = false;

    public List<DomainEvent> DomainEvents { get; set; } = [];
}