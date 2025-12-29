using System.Globalization;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Domain.Entities.Markers;
using Nullbox.Fabric.Domain.Entities.Rollups;
using Nullbox.Fabric.Domain.Markers;
using Nullbox.Fabric.Domain.Repositories.Markers;
using Nullbox.Fabric.Domain.Repositories.Rollups;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Statistics.ProcessRollups;

public class ProcessRollupsCommandHandler : IRequestHandler<ProcessRollupsCommand>
{
    private readonly ITopAliasRepository _topAliasRepository;
    private readonly ITopDomainRepository _topDomainRepository;
    private readonly IAppliedMarkerRepository _appliedMarkerRepository;

    public ProcessRollupsCommandHandler(
        ITopAliasRepository topAliasRepository,
        ITopDomainRepository topDomainRepository,
        IAppliedMarkerRepository appliedMarkerRepository)
    {
        _topAliasRepository = topAliasRepository;
        _topDomainRepository = topDomainRepository;
        _appliedMarkerRepository = appliedMarkerRepository;
    }

    public async Task Handle(ProcessRollupsCommand request, CancellationToken cancellationToken)
    {
        var receivedAtUtc = request.ReceivedAt.ToUniversalTime();
        var windows = Keys.BuildWindows(receivedAtUtc);

        foreach (var window in windows)
        {
            if (request.MailboxId is Guid mailboxId && request.AliasId is Guid aliasId)
            {
                await ApplyTopAliasAsync(
                    markerType: MarkerType.Mailbox,
                    markerGroup: MarkerGroup.TopAliases,
                    rollupType: Keys.RollupTypeMailboxAlias,
                    partitionKey: mailboxId,
                    parentId: aliasId,
                    request: request,
                    window: window,
                    cancellationToken: cancellationToken);
            }

            if (request.AccountId is Guid accountId && request.MailboxId is Guid mailboxId2)
            {
                await ApplyTopAliasAsync(
                    markerType: MarkerType.Account,
                    markerGroup: MarkerGroup.TopMailboxes,
                    rollupType: Keys.RollupTypeAccountMailbox,
                    partitionKey: accountId,
                    parentId: mailboxId2,
                    request: request,
                    window: window,
                    cancellationToken: cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(request.SenderDomain))
            {
                await ApplyTopDomainAsync(
                    markerGroup: MarkerGroup.TopSenderDomains,
                    rollupType: Keys.RollupTypeSystemSenderDomain,
                    domain: request.SenderDomain!,
                    request: request,
                    window: window,
                    cancellationToken: cancellationToken);
            }

            if (request.MailboxId is null && !string.IsNullOrWhiteSpace(request.Domain))
            {
                await ApplyTopDomainAsync(
                    markerGroup: MarkerGroup.TopTargetDomains,
                    rollupType: Keys.RollupTypeSystemTargetDomain,
                    domain: request.Domain!,
                    request: request,
                    window: window,
                    cancellationToken: cancellationToken);
            }
        }

        // Projections commit via pipeline; marker commits are immediate.
    }

    private async Task ApplyTopAliasAsync(
        MarkerType markerType,
        MarkerGroup markerGroup,
        string rollupType,
        Guid partitionKey,
        Guid parentId,
        ProcessRollupsCommand request,
        Keys.Window window,
        CancellationToken cancellationToken)
    {
        var markerKey = Keys.RollupMarkerKey(rollupType, window.Key);
        var markerId = Keys.MarkerId(rollupType, window.Key, markerType, markerGroup, MarkerStage.Created, request.Id);

        if (!await EnsureMarkerAsync(
                markerId: markerId,
                markerPartitionKey: partitionKey.ToString(),
                group: markerGroup,
                stage: MarkerStage.Created,
                key: markerKey,
                deliveryActionId: request.Id,
                cancellationToken: cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var rollupId = Keys.RollupId(rollupType, window.Key);

        var topAlias = await _topAliasRepository.FindAsync(
            r => r.Id == rollupId && r.PartitionKey == partitionKey,
            cancellationToken);

        var isNew = topAlias is null;
        if (isNew)
        {
            topAlias = new TopAlias(
                id: rollupId,
                partitionKey: partitionKey,
                windowKey: window.Key,
                windowStart: window.Start,
                windowEnd: window.End,
                updatedAt: now);

            _topAliasRepository.Add(topAlias);
        }

        ApplyTopAliasDelta(topAlias!, parentId, request);

        if (!isNew)
        {
            _topAliasRepository.Update(topAlias!);
        }
    }

    private async Task ApplyTopDomainAsync(
        MarkerGroup markerGroup,
        string rollupType,
        string domain,
        ProcessRollupsCommand request,
        Keys.Window window,
        CancellationToken cancellationToken)
    {
        var markerKey = Keys.RollupMarkerKey(rollupType, window.Key);
        var markerId = Keys.MarkerId(rollupType, window.Key, MarkerType.System, markerGroup, MarkerStage.Created, request.Id);

        if (!await EnsureMarkerAsync(
                markerId: markerId,
                markerPartitionKey: "system",
                group: markerGroup,
                stage: MarkerStage.Created,
                key: markerKey,
                deliveryActionId: request.Id,
                cancellationToken: cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var rollupId = Keys.RollupId(rollupType, window.Key);

        var topDomain = await _topDomainRepository.FindAsync(
            r => r.Id == rollupId && r.WindowKey == window.Key,
            cancellationToken);

        var isNew = topDomain is null;
        if (isNew)
        {
            topDomain = new TopDomain(
                id: rollupId,
                windowKey: window.Key,
                windowStart: window.Start,
                windowEnd: window.End,
                updatedAt: now);

            _topDomainRepository.Add(topDomain);
        }

        ApplyTopDomainDelta(topDomain!, domain, request);

        if (!isNew)
        {
            _topDomainRepository.Update(topDomain!);
        }
    }

    private static void ApplyTopAliasDelta(TopAlias topAlias, Guid parentId, ProcessRollupsCommand request)
    {
        var (dTotal, dForwarded, dDropped, dQuarantined, dDelivered, dFailed) = ComputeOutcomeDeltas(request);
        var dBandwidth = ComputeBandwidthDelta(request);

        var existing = topAlias.Items.FirstOrDefault(x => x.ParentId == parentId);

        if (existing is null)
        {
            topAlias.UpdateItem(parentId, dTotal, dForwarded, dDropped, dQuarantined, dDelivered, dFailed, dBandwidth);
            return;
        }

        topAlias.UpdateItem(
            parentId,
            existing.Total + dTotal,
            existing.Forwarded + dForwarded,
            existing.Dropped + dDropped,
            existing.Quarantined + dQuarantined,
            existing.Delivered + dDelivered,
            existing.Failed + dFailed,
            existing.Bandwidth + dBandwidth);
    }

    private static void ApplyTopDomainDelta(TopDomain topDomain, string domain, ProcessRollupsCommand request)
    {
        var (dTotal, dForwarded, dDropped, dQuarantined, dDelivered, dFailed) = ComputeOutcomeDeltas(request);
        var dBandwidth = ComputeBandwidthDelta(request);

        var existing = topDomain.Items.FirstOrDefault(x => x.Domain == domain);

        if (existing is null)
        {
            topDomain.UpdateItem(domain, dTotal, dForwarded, dDropped, dQuarantined, dDelivered, dFailed, dBandwidth);
            return;
        }

        topDomain.UpdateItem(
            domain,
            existing.Total + dTotal,
            existing.Forwarded + dForwarded,
            existing.Dropped + dDropped,
            existing.Quarantined + dQuarantined,
            existing.Delivered + dDelivered,
            existing.Failed + dFailed,
            existing.Bandwidth + dBandwidth);
    }

    private static (int total, int forwarded, int dropped, int quarantined, int delivered, int failed) ComputeOutcomeDeltas(ProcessRollupsCommand request)
    {
        var total = 1;
        var forwarded = 0;
        var dropped = 0;
        var quarantined = 0;
        var delivered = 0;
        var failed = 0;

        var providerStatusText = request.ProviderStatus?.ToString();
        if (!string.IsNullOrWhiteSpace(providerStatusText) &&
            (providerStatusText.Equals("Failed", StringComparison.OrdinalIgnoreCase) ||
             providerStatusText.Equals("Error", StringComparison.OrdinalIgnoreCase)))
        {
            failed = 1;
            return (total, forwarded, dropped, quarantined, delivered, failed);
        }

        var decision = request.DeliveryDecision.ToString();

        if (decision.Equals("Drop", StringComparison.OrdinalIgnoreCase) ||
            decision.Equals("Dropped", StringComparison.OrdinalIgnoreCase))
        {
            dropped = 1;
            return (total, forwarded, dropped, quarantined, delivered, failed);
        }

        if (decision.Equals("Quarantine", StringComparison.OrdinalIgnoreCase) ||
            decision.Equals("Quarantined", StringComparison.OrdinalIgnoreCase))
        {
            quarantined = 1;
            return (total, forwarded, dropped, quarantined, delivered, failed);
        }

        if (decision.Equals("Forward", StringComparison.OrdinalIgnoreCase) ||
            decision.Equals("Forwarded", StringComparison.OrdinalIgnoreCase))
        {
            forwarded = 1;
            return (total, forwarded, dropped, quarantined, delivered, failed);
        }

        if (decision.Equals("Deliver", StringComparison.OrdinalIgnoreCase) ||
            decision.Equals("Delivered", StringComparison.OrdinalIgnoreCase) ||
            decision.Equals("Allow", StringComparison.OrdinalIgnoreCase))
        {
            delivered = 1;
        }

        return (total, forwarded, dropped, quarantined, delivered, failed);
    }

    private static long ComputeBandwidthDelta(ProcessRollupsCommand request) => request.Size;

    private async Task<bool> EnsureMarkerAsync(
        string markerId,
        string markerPartitionKey,
        MarkerGroup group,
        MarkerStage stage,
        string key,
        Guid deliveryActionId,
        CancellationToken cancellationToken)
    {
        var existing = await _appliedMarkerRepository.FindAsync(
            m => m.Id == markerId && m.PartitionKey == markerPartitionKey,
            cancellationToken);

        if (existing is not null)
        {
            return false;
        }

        _appliedMarkerRepository.Add(new AppliedMarker
        {
            Id = markerId,
            PartitionKey = markerPartitionKey,
            DeliveryActionId = deliveryActionId,
            Stage = stage,
            Group = group,
            Key = key,
            AppliedAt = DateTimeOffset.UtcNow
        });

        await _appliedMarkerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static class Keys
    {
        public const string RollupTypeMailboxAlias = "mailbox.alias";
        public const string RollupTypeAccountMailbox = "account.mailbox";
        public const string RollupTypeSystemSenderDomain = "system.sender";
        public const string RollupTypeSystemTargetDomain = "system.target";

        public readonly record struct Window(string Key, DateTimeOffset Start, DateTimeOffset End);

        public static IReadOnlyList<Window> BuildWindows(DateTimeOffset utc)
        {
            var hourStart = new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour, 0, 0, TimeSpan.Zero);
            var hourKey = "h:" + hourStart.ToString("yyyyMMddHH", CultureInfo.InvariantCulture);

            var dayStart = new DateTimeOffset(utc.Year, utc.Month, utc.Day, 0, 0, 0, TimeSpan.Zero);
            var dayKey = "d:" + dayStart.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            var monthStart = new DateTimeOffset(utc.Year, utc.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var monthKey = "m:" + monthStart.ToString("yyyyMM", CultureInfo.InvariantCulture);

            var yearStart = new DateTimeOffset(utc.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var yearKey = "y:" + yearStart.ToString("yyyy", CultureInfo.InvariantCulture);

            // Avoid Min/Max (often problematic in DBs/serializers)
            var allKey = "a:all";
            var allStart = DateTimeOffset.UnixEpoch;
            var allEnd = DateTimeOffset.UnixEpoch;

            return new[]
            {
                new Window(hourKey, hourStart, hourStart.AddHours(1)),
                new Window(dayKey, dayStart, dayStart.AddDays(1)),
                new Window(monthKey, monthStart, monthStart.AddMonths(1)),
                new Window(yearKey, yearStart, yearStart.AddYears(1)),
                new Window(allKey, allStart, allEnd)
            };
        }

        public static string RollupId(string rollupType, string windowKey) =>
            $"r:{rollupType}|{windowKey}";

        public static string RollupMarkerKey(string rollupType, string windowKey) =>
            $"mk:{rollupType}|{windowKey}";

        public static string MarkerId(
            string rollupType,
            string windowKey,
            MarkerType markerType,
            MarkerGroup group,
            MarkerStage stage,
            Guid deliveryActionId) =>
            $"mi:{rollupType}|{windowKey}|t:{markerType}|g:{group}|s:{stage}|da:{deliveryActionId:D}";
    }
}
