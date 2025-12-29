using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Accounts;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Accounts.SynchronizeEffectiveAccountEnablement;

public class SynchronizeEffectiveAccountEnablementCommand : IRequest, ICommand
{
    public SynchronizeEffectiveAccountEnablementCommand(Guid id,
        Guid accountId,
        EnablementKind kind,
        string? productKey,
        int priority,
        DateTimeOffset? startsAt,
        DateTimeOffset? endsAt,
        int? deltaMaxMailboxes,
        int? deltaMaxAliases,
        int? deltaMaxAliasesPerMailbox,
        long? deltaMaxBandwidthBytesPerPeriod,
        Dictionary<string, string> flags,
        EnablementSource source,
        string? reason,
        bool isRevoked)
    {
        Id = id;
        AccountId = accountId;
        Kind = kind;
        ProductKey = productKey;
        Priority = priority;
        StartsAt = startsAt;
        EndsAt = endsAt;
        DeltaMaxMailboxes = deltaMaxMailboxes;
        DeltaMaxAliases = deltaMaxAliases;
        DeltaMaxAliasesPerMailbox = deltaMaxAliasesPerMailbox;
        DeltaMaxBandwidthBytesPerPeriod = deltaMaxBandwidthBytesPerPeriod;
        Flags = flags;
        Source = source;
        Reason = reason;
        IsRevoked = isRevoked;
    }

    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public EnablementKind Kind { get; set; }
    public string? ProductKey { get; set; }
    public int Priority { get; set; }
    public DateTimeOffset? StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public int? DeltaMaxMailboxes { get; set; }
    public int? DeltaMaxAliases { get; set; }
    public int? DeltaMaxAliasesPerMailbox { get; set; }
    public long? DeltaMaxBandwidthBytesPerPeriod { get; set; }
    public Dictionary<string, string> Flags { get; set; }
    public EnablementSource Source { get; set; }
    public string? Reason { get; set; }
    public bool IsRevoked { get; set; }
}