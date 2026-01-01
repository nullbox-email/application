using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Entities.Products;
using Nullbox.Fabric.Domain.Repositories.Accounts;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Accounts.SynchronizeEffectiveAccountEnablement;

public class SynchronizeEffectiveAccountEnablementCommandHandler : IRequestHandler<SynchronizeEffectiveAccountEnablementCommand>
{
    private readonly IEnablementGrantRepository _enablementGrantRepository;
    private readonly IEffectiveEnablementRepository _effectiveEnablementRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public SynchronizeEffectiveAccountEnablementCommandHandler(
        IEnablementGrantRepository enablementGrantRepository,
        IEffectiveEnablementRepository effectiveEnablementRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _enablementGrantRepository = enablementGrantRepository;
        _effectiveEnablementRepository = effectiveEnablementRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    public async Task Handle(SynchronizeEffectiveAccountEnablementCommand request, CancellationToken cancellationToken)
    {
        using var _ = _partitionKeyScope.Push(request.AccountId.ToString());

        var now = DateTimeOffset.UtcNow;

        // Load all non-revoked grants for the account, then apply "active window" filtering in-memory.
        var allGrants = await _enablementGrantRepository.FindAllAsync(
            e => e.AccountId == request.AccountId && !e.IsRevoked,
            cancellationToken);

        var activeGrants = allGrants
            .Where(g =>
                (g.StartsAt == null || g.StartsAt <= now) &&
                (g.EndsAt == null || g.EndsAt > now))
            .ToList();

        // Winning plan grant determines the effective product key and provides the base limits (non-additive).
        // Convention here: higher Priority wins. Tie-breaker: latest StartsAt, then latest Id.
        var planGrant = activeGrants
            .Where(g => g.Kind == Domain.Accounts.EnablementKind.Plan)
            .OrderByDescending(g => g.Priority)
            .ThenByDescending(g => g.StartsAt ?? DateTimeOffset.MinValue)
            .ThenByDescending(g => g.Id)
            .FirstOrDefault();

        var effectiveProductKey = planGrant?.ProductKey ?? string.Empty;

        // IMPORTANT: null means "unlimited". Do NOT coerce to 0.
        int? effectiveMaxMailboxes = null;
        int? effectiveMaxAliases = null;
        int? effectiveMaxAliasesPerMailbox = null;
        long? effectiveMaxBandwidthBytesPerPeriod = null;
        Dictionary<string, string> effectiveFlags = [];

        if (planGrant != null)
        {
            // Base values come from the winning plan grant (NOT additive).
            effectiveMaxMailboxes = planGrant.DeltaMaxMailboxes;
            effectiveMaxAliases = planGrant.DeltaMaxAliases;
            effectiveMaxAliasesPerMailbox = planGrant.DeltaMaxAliasesPerMailbox;
            effectiveMaxBandwidthBytesPerPeriod = planGrant.DeltaMaxBandwidthBytesPerPeriod;

            foreach (var flag in planGrant.Flags)
            {
                effectiveFlags[flag.Key] = flag.Value;
            }
        }

        // Apply additive grants (everything except the winning plan grant) in precedence order.
        // Convention: higher Priority applied last so it wins for flags.
        var additiveGrants = activeGrants
            .Where(g => planGrant == null || g.Id != planGrant.Id)
            .OrderBy(g => g.Priority)
            .ThenBy(g => g.StartsAt ?? DateTimeOffset.MinValue)
            .ThenBy(g => g.Id);

        foreach (var grant in additiveGrants)
        {
            // Additive semantics: if a delta exists, add it.
            // If the current effective value is null (unlimited), treat it as 0 before applying deltas,
            // because the presence of deltas implies we now have a concrete bound.
            if (grant.DeltaMaxMailboxes.HasValue)
            {
                effectiveMaxMailboxes = (effectiveMaxMailboxes ?? 0) + grant.DeltaMaxMailboxes.Value;
            }

            if (grant.DeltaMaxAliases.HasValue)
            {
                effectiveMaxAliases = (effectiveMaxAliases ?? 0) + grant.DeltaMaxAliases.Value;
            }

            if (grant.DeltaMaxAliasesPerMailbox.HasValue)
            {
                effectiveMaxAliasesPerMailbox = (effectiveMaxAliasesPerMailbox ?? 0) + grant.DeltaMaxAliasesPerMailbox.Value;
            }

            if (grant.DeltaMaxBandwidthBytesPerPeriod.HasValue)
            {
                effectiveMaxBandwidthBytesPerPeriod = (effectiveMaxBandwidthBytesPerPeriod ?? 0L) + grant.DeltaMaxBandwidthBytesPerPeriod.Value;
            }

            foreach (var flag in grant.Flags)
            {
                effectiveFlags[flag.Key] = flag.Value;
            }
        }

        // Persist effective enablements as one record per account (Id == AccountId).
        var effectiveEnablement = await _effectiveEnablementRepository.FindAsync(
            e => e.Id == request.AccountId && e.AccountId == request.AccountId,
            cancellationToken);

        if (effectiveEnablement is null)
        {
            var newEffectiveEnablement = new Domain.Entities.Accounts.EffectiveEnablement(
                id: request.AccountId,
                effectiveProductKey: effectiveProductKey,
                maxMailboxes: effectiveMaxMailboxes,
                maxAliases: effectiveMaxAliases,
                maxAliasesPerMailbox: effectiveMaxAliasesPerMailbox,
                maxBandwidthBytesPerPeriod: effectiveMaxBandwidthBytesPerPeriod,
                flags: effectiveFlags);

            _effectiveEnablementRepository.Add(newEffectiveEnablement);
        }
        else
        {
            effectiveEnablement.Update(
                effectiveProductKey: effectiveProductKey,
                maxMailboxes: effectiveMaxMailboxes,
                maxAliases: effectiveMaxAliases,
                maxAliasesPerMailbox: effectiveMaxAliasesPerMailbox,
                maxBandwidthBytesPerPeriod: effectiveMaxBandwidthBytesPerPeriod,
                flags: effectiveFlags);

            _effectiveEnablementRepository.Update(effectiveEnablement);
        }
    }
}
