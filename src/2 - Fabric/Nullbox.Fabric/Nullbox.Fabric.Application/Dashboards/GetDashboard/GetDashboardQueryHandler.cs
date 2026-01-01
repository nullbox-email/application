using System.Globalization;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Markers;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Activities;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Statistics;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.Dashboards.GetDashboard;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private const int DefaultRecentTake = 200;

    private readonly IAliasRepository _aliasRepository;
    private readonly IRecentDeliveryActionRepository _recentDeliveryActionRepository;
    private readonly ITrafficStatisticRepository _trafficStatisticRepository;
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDashboardQueryHandler(
        IAliasRepository aliasRepository,
        IRecentDeliveryActionRepository recentDeliveryActionRepository,
        ITrafficStatisticRepository trafficStatisticRepository,
        IMailboxRepository mailboxRepository,
        IAccountUserMapRepository accountUserMapRepository,
        ICurrentUserService currentUserService)
    {
        _aliasRepository = aliasRepository;
        _recentDeliveryActionRepository = recentDeliveryActionRepository;
        _trafficStatisticRepository = trafficStatisticRepository;
        _mailboxRepository = mailboxRepository;
        _accountUserMapRepository = accountUserMapRepository;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserService.GetAsync();

        if (currentUser is null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        if (!Guid.TryParse(currentUser.Id, out var userId))
        {
            throw new ForbiddenAccessException("Invalid user ID.");
        }

        var account = await _accountUserMapRepository.FindAsync(a => a.PartitionKey == userId, cancellationToken);
        if (account is null)
        {
            throw new ForbiddenAccessException("User does not have access to any account.");
        }

        // Placeholder: for now, infer scope from request payload.
        // Later: add request.Level (MarkerType) and switch on that.
        //
        // Expected future:
        // - request.Level == MarkerType.Account => partitionId = account.Id; mailbox/alias optional
        // - request.Level == MarkerType.Mailbox => partitionId = request.MailboxId; alias optional
        // - request.Level == MarkerType.Alias   => partitionId = request.Id
        var level = PlaceholderDetermineLevel(request);

        Guid partitionId;
        Guid? mailboxId = null;
        Guid? aliasId = null;

        switch (level)
        {
            case MarkerType.Account:
                partitionId = account.Id;
                break;

            case MarkerType.Mailbox:
                mailboxId = request.MailboxId;
                if (mailboxId == Guid.Empty)
                    throw new NotFoundException("MailboxId is required for mailbox-level dashboard.");
                partitionId = mailboxId.Value;
                break;

            case MarkerType.Alias:
                mailboxId = request.MailboxId;
                aliasId = request.AliasId;
                if (mailboxId == Guid.Empty)
                    throw new NotFoundException("MailboxId is required for alias-level dashboard.");
                if (aliasId == Guid.Empty)
                    throw new NotFoundException("AliasId is required for alias-level dashboard.");
                partitionId = aliasId.Value;
                break;

            default:
                throw new InvalidOperationException($"Unsupported dashboard level '{level}'.");
        }

        // Authorization checks only when needed for the requested level.
        // Account-level: already authorized via account map.
        if (level == MarkerType.Mailbox || level == MarkerType.Alias)
        {
            var mailbox = await _mailboxRepository.FindAsync(
                m => m.Id == mailboxId!.Value && m.AccountId == account.Id,
                cancellationToken);

            if (mailbox is null)
            {
                throw new NotFoundException($"Could not find Mailbox '{mailboxId}'");
            }

            if (level == MarkerType.Alias)
            {
                var alias = await _aliasRepository.FindAsync(
                    a => a.Id == aliasId!.Value && a.MailboxId == mailbox.Id,
                    cancellationToken);

                if (alias is null)
                {
                    throw new NotFoundException($"Could not find Alias '{aliasId}' for Mailbox '{mailbox.Id}'");
                }
            }
        }

        // Build expected buckets (id + start timestamp), newest -> oldest.
        var expectedBuckets = new List<(string Id, DateTimeOffset BucketStartAt)>(capacity: request.Number);

        for (var i = 0; i < request.Number; i++)
        {
            if (request.Type == DashboardType.Daily)
            {
                var dayStartUtc = FloorToDayUtc(DateTimeOffset.UtcNow).AddDays(-i);
                expectedBuckets.Add((Keys.BucketId(level, Keys.DayBucketKey(dayStartUtc)), dayStartUtc));
            }
            else if (request.Type == DashboardType.Hourly)
            {
                var hourStartUtc = FloorToHourUtc(DateTimeOffset.UtcNow).AddHours(-i);
                expectedBuckets.Add((Keys.BucketId(level, Keys.HourBucketKey(hourStartUtc)), hourStartUtc));
            }
        }

        var bucketIds = expectedBuckets.Select(b => b.Id).ToList();

        // NOTE: after unifying stats, Id is now "t:{MarkerType}|b:..." or "t:{MarkerType}|d:..."
        var trafficStatistics = await _trafficStatisticRepository.FindAllAsync(
            s => s.PartitionKey == partitionId && bucketIds.Contains(s.Id),
            cancellationToken);

        var byId = trafficStatistics.ToDictionary(s => s.Id);

        var chartData = expectedBuckets
            .Select(b =>
            {
                if (byId.TryGetValue(b.Id, out var s))
                {
                    return TrafficStatisticDto.Create(
                        timestamp: s.BucketStartAt,
                        total: s.Total,
                        forwarded: s.Forwarded,
                        dropped: s.Dropped,
                        quarantined: s.Quarantined,
                        delivered: s.Delivered,
                        failed: s.Failed,
                        bandwidth: s.Bandwidth);
                }

                return TrafficStatisticDto.Create(
                    timestamp: b.BucketStartAt,
                    total: 0,
                    forwarded: 0,
                    dropped: 0,
                    quarantined: 0,
                    delivered: 0,
                    failed: 0,
                    bandwidth: 0);
            })
            .OrderByDescending(x => x.Timestamp)
            .ToList();

        // Recent messages: use the same partitionId (alias/mailbox/account)
        var windowStartUtc = expectedBuckets.Min(b => b.BucketStartAt);
        var windowEndUtc = DateTimeOffset.UtcNow;

        var recentMessages = await _recentDeliveryActionRepository.FindAllAsync(
            x => x.PartitionKey == partitionId
                 && x.ReceivedAt >= windowStartUtc
                 && x.ReceivedAt <= windowEndUtc,
            q => q
                .OrderByDescending(x => x.ReceivedAt)
                .Take(DefaultRecentTake),
            cancellationToken);

        var messageData = recentMessages
            .Select(m =>
            {
                var uiDecision = MapUiDeliveryDecision(
                    rawDecision: m.DeliveryDecision,
                    providerStatus: m.ProviderStatus,
                    forwardTo: m.ForwardTo,
                    forwardFrom: m.ForwardFrom);

                var uiReason = m.DeliveryDecision switch
                {
                    Domain.Deliveries.DeliveryDecision.Drop => m.DropReason.Message,
                    Domain.Deliveries.DeliveryDecision.Quarantine => m.QuarantineReason.Message,
                    Domain.Deliveries.DeliveryDecision.Forward => null,
                    _ => null
                };

                return DeliveryActionDto.Create(
                    id: m.Id,
                    receivedAt: m.ReceivedAt,
                    senderDisplay: m.SenderDisplay,
                    senderDomain: m.SenderDomain,
                    subject: m.Subject,
                    messageOutcome: uiDecision,
                    reason: uiReason,
                    providerStatus: NormalizeProviderStatus(m.ProviderStatus),
                    hasAttachments: m.HasAttachments,
                    attachmentsCount: m.AttachmentsCount,
                    size: m.Size);
            })
            .ToList();

        return new DashboardDto
        {
            Chart = chartData,
            Messages = messageData
        };
    }

    private static MarkerType PlaceholderDetermineLevel(GetDashboardQuery request)
    {
        if (request.MailboxId.HasValue && request.MailboxId != Guid.Empty)
        {
            if (request.AliasId.HasValue && request.AliasId != Guid.Empty)
            {
                return MarkerType.Alias;
            }

            return MarkerType.Mailbox;
        }

        return MarkerType.Account;
    }

    private static string MapUiDeliveryDecision(
        Nullbox.Fabric.Domain.Deliveries.DeliveryDecision rawDecision,
        object? providerStatus,
        string? forwardTo,
        string? forwardFrom)
    {
        switch (rawDecision)
        {
            case Nullbox.Fabric.Domain.Deliveries.DeliveryDecision.Drop:
                return "Dropped";

            case Nullbox.Fabric.Domain.Deliveries.DeliveryDecision.Quarantine:
                return "Quarantined";

            case Nullbox.Fabric.Domain.Deliveries.DeliveryDecision.Forward:
                {
                    var isForwarding = !string.IsNullOrWhiteSpace(forwardTo) || !string.IsNullOrWhiteSpace(forwardFrom);
                    if (isForwarding) return "Forwarded";

                    var ps = NormalizeProviderStatus(providerStatus);

                    if (ps.Equals("Succeeded", StringComparison.OrdinalIgnoreCase))
                        return "Delivered";

                    return "Unknown";
                }

            default:
                return "Unknown";
        }
    }

    private static string? NormalizeProviderStatus(object? providerStatus)
    {
        if (providerStatus is null) return "Unknown";

        if (providerStatus is string s)
        {
            s = s.Trim();
            if (s.Equals("Pending", StringComparison.OrdinalIgnoreCase)) return "Pending";
            if (s.Equals("Succeeded", StringComparison.OrdinalIgnoreCase)) return "Succeeded";
            if (s.Equals("Failed", StringComparison.OrdinalIgnoreCase)) return "Failed";
            return "Unknown";
        }

        var name = providerStatus.ToString() ?? "Unknown";
        return NormalizeProviderStatus(name);
    }

    private static DateTimeOffset FloorToHourUtc(DateTimeOffset utc)
    {
        utc = utc.ToUniversalTime();
        return new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour, 0, 0, TimeSpan.Zero);
    }

    private static DateTimeOffset FloorToDayUtc(DateTimeOffset utc)
    {
        utc = utc.ToUniversalTime();
        return new DateTimeOffset(utc.Year, utc.Month, utc.Day, 0, 0, 0, TimeSpan.Zero);
    }

    private static class Keys
    {
        public static string HourBucketKey(DateTimeOffset utc) =>
            "b:" + utc.ToString("yyyyMMddHH", CultureInfo.InvariantCulture);

        public static string DayBucketKey(DateTimeOffset utc) =>
            "d:" + utc.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        // New: stat ids now include the marker type prefix used by your unified statistics writes.
        // Example: "t:Alias|b:2025122414" or "t:Account|d:20251224"
        public static string BucketId(MarkerType markerType, string timeBucketKey) =>
            $"t:{markerType}|{timeBucketKey}";
    }
}
