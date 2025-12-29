using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Events.Accounts;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Accounts;

public partial class EnablementGrant
{
    public EnablementGrant(Guid accountId,
            string? productKey,
            DateTimeOffset? startsAt,
            DateTimeOffset? endsAt,
            int? deltaMaxMailboxes,
            int? deltaMaxAliases,
            int? deltaMaxAliasesPerMailbox,
            long? deltaMaxBandwidthBytesPerPeriod,
            Dictionary<string, string> flags)
    {
        Kind = Domain.Accounts.EnablementKind.Plan;

        AccountId = accountId;
        ProductKey = productKey;
        StartsAt = startsAt;
        EndsAt = endsAt;
        DeltaMaxMailboxes = deltaMaxMailboxes;
        DeltaMaxAliases = deltaMaxAliases;
        DeltaMaxAliasesPerMailbox = deltaMaxAliasesPerMailbox;
        DeltaMaxBandwidthBytesPerPeriod = deltaMaxBandwidthBytesPerPeriod;
        Flags = flags;
        DomainEvents.Add(new EnablementGrantCreatedDomainEvent(
            enablementGrant: this));
    }

    public void Revoke()
    {
        if (IsRevoked)
        {
            throw new InvalidOperationException("Enablement grant is already revoked.");
        }

        IsRevoked = true;
    }
}