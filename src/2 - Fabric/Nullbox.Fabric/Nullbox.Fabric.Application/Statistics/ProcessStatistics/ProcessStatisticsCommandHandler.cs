using System.Globalization;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Domain.Entities.Markers;
using Nullbox.Fabric.Domain.Entities.Statistics;
using Nullbox.Fabric.Domain.Markers;
using Nullbox.Fabric.Domain.Repositories.Markers;
using Nullbox.Fabric.Domain.Repositories.Statistics;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Statistics.ProcessStatistics;

public class ProcessStatisticsCommandHandler : IRequestHandler<ProcessStatisticsCommand>
{
    private readonly ITrafficStatisticRepository _trafficStatisticRepository;
    private readonly IAppliedMarkerRepository _appliedMarkerRepository;

    public ProcessStatisticsCommandHandler(
        ITrafficStatisticRepository trafficStatisticRepository,
        IAppliedMarkerRepository appliedMarkerRepository)
    {
        _trafficStatisticRepository = trafficStatisticRepository;
        _appliedMarkerRepository = appliedMarkerRepository;
    }

    public async Task Handle(ProcessStatisticsCommand request, CancellationToken cancellationToken)
    {
        var receivedAtUtc = request.ReceivedAt.ToUniversalTime();
        var windows = Keys.BuildWindows(receivedAtUtc);

        foreach (var window in windows)
        {
            // System scope (PartitionKey = Guid.Empty)
            await ApplyTrafficCreatedAsync(
                request: request,
                markerType: MarkerType.System,
                scopePartitionGuid: Guid.Empty,
                markerPartitionKey: "system",
                bucketKey: Keys.BucketKey(MarkerType.System, window.Key),
                bucketStart: window.Start,
                cancellationToken: cancellationToken);

            if (request.AliasId is Guid aliasId)
            {
                await ApplyTrafficCreatedAsync(
                    request: request,
                    markerType: MarkerType.Alias,
                    scopePartitionGuid: aliasId,
                    markerPartitionKey: aliasId.ToString(),
                    bucketKey: Keys.BucketKey(MarkerType.Alias, window.Key),
                    bucketStart: window.Start,
                    cancellationToken: cancellationToken);
            }

            if (request.MailboxId is Guid mailboxId)
            {
                await ApplyTrafficCreatedAsync(
                    request: request,
                    markerType: MarkerType.Mailbox,
                    scopePartitionGuid: mailboxId,
                    markerPartitionKey: mailboxId.ToString(),
                    bucketKey: Keys.BucketKey(MarkerType.Mailbox, window.Key),
                    bucketStart: window.Start,
                    cancellationToken: cancellationToken);
            }

            if (request.AccountId is Guid accountId)
            {
                await ApplyTrafficCreatedAsync(
                    request: request,
                    markerType: MarkerType.Account,
                    scopePartitionGuid: accountId,
                    markerPartitionKey: accountId.ToString(),
                    bucketKey: Keys.BucketKey(MarkerType.Account, window.Key),
                    bucketStart: window.Start,
                    cancellationToken: cancellationToken);
            }
        }

        // No SaveChanges here; pipeline commits once.
        // If you need markers to be a hard gate under concurrency, commit markers immediately.
    }

    private async Task ApplyTrafficCreatedAsync(
        ProcessStatisticsCommand request,
        MarkerType markerType,
        Guid scopePartitionGuid,
        string markerPartitionKey,
        string bucketKey,
        DateTimeOffset bucketStart,
        CancellationToken cancellationToken)
    {
        // Marker: one per delivery action per bucket (hour/day/month/year/all-time) per scope.
        var markerId = Keys.MarkerId(markerType, MarkerGroup.Traffic, MarkerStage.Created, bucketKey, request.Id);

        if (!await TryAddMarkerAsync(
                markerId: markerId,
                markerPartitionKey: markerPartitionKey,
                group: MarkerGroup.Traffic,
                stage: MarkerStage.Created,
                key: bucketKey,
                deliveryActionId: request.Id,
                cancellationToken: cancellationToken))
        {
            return;
        }

        // DISTINCT COUNTS (Aliases / Mailboxes)
        // These are "active per period" counts: first time a given alias/mailbox is observed in this bucket+scope.
        var (dAliases, dMailboxes) = await ComputeDistinctDeltasAsync(
            request: request,
            markerType: markerType,
            markerPartitionKey: markerPartitionKey,
            bucketKey: bucketKey,
            cancellationToken: cancellationToken);

        var statisticId = bucketKey; // unique across type+bucket
        var trafficStatistic = await _trafficStatisticRepository.FindAsync(
            s => s.Id == statisticId && s.PartitionKey == scopePartitionGuid,
            cancellationToken);

        var now = DateTimeOffset.UtcNow;

        if (trafficStatistic is null)
        {
            var (dTotal, dForwarded, dDropped, dQuarantined, dDelivered, dFailed) = ComputeOutcomeDeltas(request);
            var dBandwidth = ComputeBandwidthDelta(request);

            trafficStatistic = new TrafficStatistic(
                id: statisticId,
                partitionKey: scopePartitionGuid,
                bucketKey: bucketKey,
                bucketStartAt: bucketStart,
                total: dTotal,
                forwarded: dForwarded,
                dropped: dDropped,
                quarantined: dQuarantined,
                delivered: dDelivered,
                failed: dFailed,
                bandwidth: dBandwidth,
                aliases: dAliases,
                mailboxes: dMailboxes,
                lastCreatedEventId: request.Id.ToString("D"),
                lastDecisionedEventId: null,
                lastCompletedEventId: null,
                updatedAt: now);

            _trafficStatisticRepository.Add(trafficStatistic);
            return;
        }

        // Update existing
        var (total, forwarded, dropped, quarantined, delivered, failed) = ApplyOutcomeDeltaToExisting(trafficStatistic, request);
        var bandwidth = trafficStatistic.Bandwidth + ComputeBandwidthDelta(request);

        var aliases = trafficStatistic.Aliases + dAliases;
        var mailboxes = trafficStatistic.Mailboxes + dMailboxes;

        trafficStatistic.Update(
            total: total,
            forwarded: forwarded,
            dropped: dropped,
            quarantined: quarantined,
            delivered: delivered,
            failed: failed,
            bandwidth: bandwidth,
            aliases: aliases,
            mailboxes: mailboxes,
            lastCreatedEventId: request.Id.ToString("D"),
            lastDecisionedEventId: trafficStatistic.LastDecisionedEventId,
            lastCompletedEventId: trafficStatistic.LastCompletedEventId,
            updatedAt: now);

        _trafficStatisticRepository.Update(trafficStatistic);
    }

    private async Task<(int dAliases, int dMailboxes)> ComputeDistinctDeltasAsync(
        ProcessStatisticsCommand request,
        MarkerType markerType,
        string markerPartitionKey,
        string bucketKey,
        CancellationToken cancellationToken)
    {
        var dAliases = 0;
        var dMailboxes = 0;

        // Track distinct aliases when an AliasId is present.
        // Note: this is an "active aliases" count (aliases with traffic), not "aliases created".
        if (request.AliasId is Guid aliasId)
        {
            var distinctAliasMarkerId = Keys.DistinctMarkerId(
                type: markerType,
                group: MarkerGroup.Traffic,
                stage: MarkerStage.Distinct,
                key: bucketKey,
                entityDiscriminator: "a",
                entityId: aliasId);

            if (await TryAddMarkerAsync(
                    markerId: distinctAliasMarkerId,
                    markerPartitionKey: markerPartitionKey,
                    group: MarkerGroup.Traffic,
                    stage: MarkerStage.Distinct,
                    key: bucketKey,
                    deliveryActionId: request.Id,
                    cancellationToken: cancellationToken))
            {
                dAliases = 1;
            }
        }

        // Track distinct mailboxes when a MailboxId is present.
        if (request.MailboxId is Guid mailboxId)
        {
            var distinctMailboxMarkerId = Keys.DistinctMarkerId(
                type: markerType,
                group: MarkerGroup.Traffic,
                stage: MarkerStage.Distinct,
                key: bucketKey,
                entityDiscriminator: "m",
                entityId: mailboxId);

            if (await TryAddMarkerAsync(
                    markerId: distinctMailboxMarkerId,
                    markerPartitionKey: markerPartitionKey,
                    group: MarkerGroup.Traffic,
                    stage: MarkerStage.Distinct,
                    key: bucketKey,
                    deliveryActionId: request.Id,
                    cancellationToken: cancellationToken))
            {
                dMailboxes = 1;
            }
        }

        return (dAliases, dMailboxes);
    }

    private static (int total, int forwarded, int dropped, int quarantined, int delivered, int failed) ApplyOutcomeDeltaToExisting(
        TrafficStatistic existing,
        ProcessStatisticsCommand request)
    {
        var (dTotal, dForwarded, dDropped, dQuarantined, dDelivered, dFailed) = ComputeOutcomeDeltas(request);

        return (
            total: existing.Total + dTotal,
            forwarded: existing.Forwarded + dForwarded,
            dropped: existing.Dropped + dDropped,
            quarantined: existing.Quarantined + dQuarantined,
            delivered: existing.Delivered + dDelivered,
            failed: existing.Failed + dFailed);
    }

    private static (int total, int forwarded, int dropped, int quarantined, int delivered, int failed) ComputeOutcomeDeltas(ProcessStatisticsCommand request)
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

    private static long ComputeBandwidthDelta(ProcessStatisticsCommand request) => request.Size;

    private async Task<bool> TryAddMarkerAsync(
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

        return true;
    }

    private static class Keys
    {
        public readonly record struct Window(string Key, DateTimeOffset Start, DateTimeOffset End);

        public static IReadOnlyList<Window> BuildWindows(DateTimeOffset utc)
        {
            // Hourly
            var hourStart = new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour, 0, 0, TimeSpan.Zero);
            var hourKey = "h:" + hourStart.ToString("yyyyMMddHH", CultureInfo.InvariantCulture);

            // Daily
            var dayStart = new DateTimeOffset(utc.Year, utc.Month, utc.Day, 0, 0, 0, TimeSpan.Zero);
            var dayKey = "d:" + dayStart.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            // Monthly
            var monthStart = new DateTimeOffset(utc.Year, utc.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var monthKey = "m:" + monthStart.ToString("yyyyMM", CultureInfo.InvariantCulture);

            // Yearly
            var yearStart = new DateTimeOffset(utc.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var yearKey = "y:" + yearStart.ToString("yyyy", CultureInfo.InvariantCulture);

            // All time (avoid Min/Max)
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

        public static string BucketKey(MarkerType markerType, string timeBucketKey) =>
            $"t:{markerType}|{timeBucketKey}";

        public static string MarkerId(MarkerType type, MarkerGroup group, MarkerStage stage, string key, Guid deliveryActionId) =>
            $"t:{type}|g:{group}|k:{key}|s:{stage}|da:{deliveryActionId:D}";

        public static string DistinctMarkerId(
            MarkerType type,
            MarkerGroup group,
            MarkerStage stage,
            string key,
            string entityDiscriminator,
            Guid entityId) =>
            $"t:{type}|g:{group}|k:{key}|s:{stage}|{entityDiscriminator}:{entityId:D}";
    }
}
