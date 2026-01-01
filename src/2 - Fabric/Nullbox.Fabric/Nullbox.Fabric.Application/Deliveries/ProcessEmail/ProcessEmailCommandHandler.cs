using System.Diagnostics;
using System.Diagnostics.Metrics;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Aliases;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Deliveries;
using Nullbox.Fabric.Domain.Entities.Aliases;
using Nullbox.Fabric.Domain.Entities.Deliveries;
using Nullbox.Fabric.Domain.Markers;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Deliveries;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Statistics;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Deliveries.ProcessEmail;

public class ProcessEmailCommandHandler : IRequestHandler<ProcessEmailCommand, DeliveryDecisionDto>
{
    // Telemetry primitives (stable names)
    private static readonly ActivitySource ActivitySource = new("Nullbox.Fabric.ProcessEmail");
    private static readonly Meter Meter = new("Nullbox.Fabric", "1.0.0");

    private static readonly Counter<long> TotalMessages =
        Meter.CreateCounter<long>("email.messages.total", unit: "{message}", description: "Total messages processed");

    private static readonly Counter<long> ForwardedMessages =
        Meter.CreateCounter<long>("email.messages.forwarded", unit: "{message}", description: "Messages forwarded");

    private static readonly Counter<long> DroppedMessages =
        Meter.CreateCounter<long>("email.messages.dropped", unit: "{message}", description: "Messages dropped");

    private static readonly Counter<long> QuarantinedMessages =
        Meter.CreateCounter<long>("email.messages.quarantined", unit: "{message}", description: "Messages quarantined");

    private static readonly Histogram<double> ProcessingDurationMs =
        Meter.CreateHistogram<double>("email.processing.duration", unit: "ms", description: "ProcessEmail handler duration");

    private readonly IMailboxMapRepository _mailboxMapRepository;
    private readonly IAliasMapRepository _aliasMapRepository;
    private readonly IAliasRepository _aliasRepository;
    private readonly IDeliveryActionRepository _deliveryActionRepository;
    private readonly IAliasRuleRepository _aliasRuleRepository;
    private readonly IAliasSenderRepository _aliasSenderRepository;
    private readonly ITrafficStatisticRepository _trafficStatisticRepository;
    private readonly IEffectiveEnablementRepository _effectiveEnablementRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;
    private readonly ILogger<ProcessEmailCommandHandler> _logger;

    public ProcessEmailCommandHandler(
        IMailboxMapRepository mailboxMapRepository,
        IAliasMapRepository aliasMapRepository,
        IAliasRepository aliasRepository,
        IDeliveryActionRepository deliveryActionRepository,
        IAliasRuleRepository aliasRuleRepository,
        IAliasSenderRepository aliasSenderRepository,
        ITrafficStatisticRepository trafficStatisticRepository,
        IEffectiveEnablementRepository effectiveEnablementRepository,
        IPartitionKeyScope partitionKeyScope,
        ILogger<ProcessEmailCommandHandler> logger)
    {
        _mailboxMapRepository = mailboxMapRepository;
        _aliasMapRepository = aliasMapRepository;
        _aliasRepository = aliasRepository;
        _deliveryActionRepository = deliveryActionRepository;
        _aliasRuleRepository = aliasRuleRepository;
        _aliasSenderRepository = aliasSenderRepository;
        _trafficStatisticRepository = trafficStatisticRepository;
        _effectiveEnablementRepository = effectiveEnablementRepository;
        _partitionKeyScope = partitionKeyScope;
        _logger = logger;
    }

    private static string HourBucket(DateTimeOffset utc)
        => utc.ToUniversalTime().ToString("yyyyMMddHH");

    private static string BuildPartitionKey(Guid? aliasId, Guid? mailboxId, string? domain, DateTimeOffset receivedAtUtc)
    {
        var b = HourBucket(receivedAtUtc);

        if (aliasId is Guid a && a != Guid.Empty)
            return $"a:{a}:t:{b}";

        if (mailboxId is Guid m && m != Guid.Empty)
            return $"m:{m}:t:{b}";

        var d = (domain ?? "unknown").Trim().ToLowerInvariant();
        return $"d:{d}:t:{b}";
    }

    private static string NormalizeSubject(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return string.Empty;
        return string.Join(' ', s.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private static string NormalizeEmail(string? s)
        => (s ?? string.Empty).Trim().ToLowerInvariant();

    private static string NormalizeHost(string? s)
        => (s ?? string.Empty).Trim().ToLowerInvariant();

    private static string BuildSenderEmail(string senderDisplay, string senderDomain)
    {
        // If senderDisplay already includes '@', treat it as the full sender address.
        if (!string.IsNullOrWhiteSpace(senderDisplay) && senderDisplay.Contains('@'))
            return senderDisplay;

        // Fallback: combine display (assumed local) + senderDomain (assumed domain/host)
        if (!string.IsNullOrWhiteSpace(senderDisplay) && !string.IsNullOrWhiteSpace(senderDomain))
            return $"{senderDisplay}@{senderDomain}";

        return senderDisplay;
    }

    private static bool IsSuspiciousSender(string senderEmailNormalized, string senderDomainNormalized)
    {
        // Minimal heuristic placeholder:
        // - Any non-ascii characters in domain
        // - Punycode domain
        if (string.IsNullOrWhiteSpace(senderDomainNormalized))
            return true;

        if (senderDomainNormalized.StartsWith("xn--", StringComparison.Ordinal))
            return true;

        foreach (var ch in senderDomainNormalized)
        {
            if (ch > 127)
                return true;
        }

        // If the email itself contains non-ascii, treat as suspicious (conservative).
        foreach (var ch in senderEmailNormalized)
        {
            if (ch > 127)
                return true;
        }

        return false;
    }

    private static EmailProcessingAction MapDecisionToAction(DeliveryDecision decision)
        => decision switch
        {
            DeliveryDecision.Forward => EmailProcessingAction.Forward,
            DeliveryDecision.Drop => EmailProcessingAction.Drop,
            DeliveryDecision.Quarantine => EmailProcessingAction.Quarantine,
            _ => EmailProcessingAction.Quarantine
        };

    // Cosmos-friendly deterministic IDs (avoid collisions when sharing a container)
    private static string RuleId(string senderEmailNormalized) => $"r:{senderEmailNormalized}";
    private static string SenderId(string senderEmailNormalized) => $"s:{senderEmailNormalized}";

    // Matches ProcessStatistics.Keys: BucketKey(MarkerType.Mailbox, "a:all") => $"t:{markerType}|{timeBucketKey}"
    private static string AllTimeMailboxBucketKey() => $"t:{MarkerType.Mailbox}|a:all";

    // Monthly period: BucketKey(MarkerType.Account, "m:yyyyMM") in ProcessStatistics
    private static string MonthlyAccountBucketKey(DateTimeOffset receivedAtUtc)
    {
        var utc = receivedAtUtc.ToUniversalTime();
        var monthKey = "m:" + utc.ToString("yyyyMM");
        return $"t:{MarkerType.Account}|{monthKey}";
    }

    private async Task<int> GetCurrentAliasCountForMailboxAsync(Guid mailboxId, CancellationToken cancellationToken)
    {
        // NEW: use TrafficStatistic.Aliases from mailbox all-time bucket.
        // This represents distinct aliases observed (active) for this mailbox over all time.
        var statId = AllTimeMailboxBucketKey();

        var stat = await _trafficStatisticRepository.FindAsync(
            s => s.Id == statId && s.PartitionKey == mailboxId,
            cancellationToken);

        return stat?.Aliases ?? 0;
    }

    private async Task<int?> GetMaxAliasesPerMailboxAsync(Guid accountId, CancellationToken cancellationToken)
    {
        // null => unlimited
        var effective = await _effectiveEnablementRepository.FindAsync(
            e => e.Id == accountId && e.AccountId == accountId,
            cancellationToken);

        return effective?.MaxAliasesPerMailbox;
    }

    private async Task<long> GetCurrentBandwidthForAccountMonthAsync(Guid accountId, DateTimeOffset receivedAtUtc, CancellationToken cancellationToken)
    {
        // Bandwidth is tracked on TrafficStatistic.Bandwidth
        // Monthly period is keyed as: bucketKey = $"t:{MarkerType.Account}|m:yyyyMM"
        var statId = MonthlyAccountBucketKey(receivedAtUtc);

        var stat = await _trafficStatisticRepository.FindAsync(
            s => s.Id == statId && s.PartitionKey == accountId,
            cancellationToken);

        return stat?.Bandwidth ?? 0L;
    }

    private async Task<long?> GetMaxBandwidthBytesPerPeriodAsync(Guid accountId, CancellationToken cancellationToken)
    {
        // null => unlimited
        var effective = await _effectiveEnablementRepository.FindAsync(
            e => e.Id == accountId && e.AccountId == accountId,
            cancellationToken);

        return effective?.MaxBandwidthBytesPerPeriod;
    }

    public async Task<DeliveryDecisionDto> Handle(ProcessEmailCommand request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        // Normalize inputs (store normalized values in the log)
        var aliasLocal = (request.Alias ?? string.Empty).Trim().ToLowerInvariant();
        var domain = (request.Domain ?? string.Empty).Trim().ToLowerInvariant();
        var routingKey = (request.RoutingKey ?? string.Empty).Trim().ToLowerInvariant();

        var receivedAtUtc = request.ReceivedAtUtc;
        if (receivedAtUtc == default) receivedAtUtc = DateTimeOffset.UtcNow;

        var source = string.IsNullOrWhiteSpace(request.Source) ? "cloudflare-email-routing" : request.Source.Trim();

        var senderDisplay = (request.Sender ?? string.Empty).Trim();
        var senderDomain = (request.SenderDomain ?? string.Empty).Trim().ToLowerInvariant();

        var recipientDisplay = (request.Recipient ?? string.Empty).Trim();
        var recipientDomain = (request.RecipientDomain ?? string.Empty).Trim().ToLowerInvariant();

        var messageId = string.IsNullOrWhiteSpace(request.MessageId) ? null : request.MessageId.Trim();

        var subject = NormalizeSubject(request.Subject);
        var subjectHash = (request.SubjectHash ?? string.Empty).Trim();

        var hasAttachments = request.HasAttachments;
        var attachmentsCount = request.AttachmentsCount < 0 ? 0 : request.AttachmentsCount;
        var size = request.Size < 0 ? 0 : request.Size;

        var fullyQualifiedAlias = $"{aliasLocal}@{domain}";

        // Trace span
        using var activity = ActivitySource.StartActivity("ProcessEmail", ActivityKind.Internal);
        activity?.SetTag("email.source", source);
        activity?.SetTag("email.domain", domain); // remove if cardinality is too high
        activity?.SetTag("email.routing_key", routingKey); // remove if high-cardinality
        activity?.SetTag("email.has_attachments", hasAttachments);
        activity?.SetTag("email.attachments_count", attachmentsCount);
        activity?.SetTag("email.size", size);

        // Metrics: total
        TotalMessages.Add(1,
            new("source", source),
            new("domain", domain));

        EmailProcessingAction action = EmailProcessingAction.Drop;
        string? forwardTo = null;
        string? forwardFrom = null;

        Guid? aliasId = null;
        Guid? mailboxId = null;
        Guid? accountId = null;

        // Track: alias auto-created during this request
        var autoCreatedAliasThisRequest = false;

        // For outcome telemetry/logging
        DropReason? dropReason = null;
        QuarantineReason? quarantineReason = null;

        // NEW: track whether bandwidth gate blocked forwarding (for consistent messaging)
        var bandwidthGateBlocked = false;
        long? bandwidthCurrentBytes = null;
        long? bandwidthProjectedBytes = null;
        long? bandwidthMaxBytes = null;

        // Build sender fields once (used for logging even when recipient invalid)
        var senderEmailNormalized = NormalizeEmail(BuildSenderEmail(senderDisplay, senderDomain));
        var senderDomainNormalized = NormalizeHost(senderDomain);

        try
        {
            // Resolve alias map
            var aliasMap = await _aliasMapRepository.FindByIdAsync(fullyQualifiedAlias, cancellationToken: cancellationToken);

            // Resolve mailbox map (always attempt; needed for alias-missing case)
            var mailbox = aliasMap == null
                ? await _mailboxMapRepository.FindByIdAsync(domain, cancellationToken: cancellationToken)
                : null;

            // Determine high-level routing decision
            if (aliasMap == null && mailbox == null)
            {
                action = EmailProcessingAction.Drop;
                dropReason = DropReason.InvalidRecipient;
            }
            else if (aliasMap == null)
            {
                // Mailbox known, alias missing
                mailboxId = mailbox!.MailboxId;
                accountId = mailbox.AccountId;

                // Alias count gate: use TrafficStatistic.Aliases on mailbox all-time bucket.
                var currentAliasCount = await GetCurrentAliasCountForMailboxAsync(mailbox.MailboxId, cancellationToken);
                var maxAliasesPerMailbox = await GetMaxAliasesPerMailboxAsync(mailbox.AccountId, cancellationToken);

                // null => unlimited
                var underAliasLimit = maxAliasesPerMailbox is null || currentAliasCount < maxAliasesPerMailbox.Value;

                // Monthly bandwidth gate: TrafficStatistic.Bandwidth on account monthly bucket + effective enablement
                var currentBandwidthThisMonth = await GetCurrentBandwidthForAccountMonthAsync(mailbox.AccountId, receivedAtUtc, cancellationToken);
                var maxBandwidthBytesPerPeriod = await GetMaxBandwidthBytesPerPeriodAsync(mailbox.AccountId, cancellationToken);

                // null => unlimited
                var projectedBandwidth = checked(currentBandwidthThisMonth + (long)size);
                var underBandwidthLimit = maxBandwidthBytesPerPeriod is null || projectedBandwidth <= maxBandwidthBytesPerPeriod.Value;

                var canAutoCreate = mailbox.AutoCreateAlias && underAliasLimit && underBandwidthLimit;

                if (canAutoCreate)
                {
                    var newAlias = new Alias(
                        mailboxId: mailbox.MailboxId,
                        accountId: mailbox.AccountId,
                        name: aliasLocal,
                        localPart: aliasLocal);

                    _aliasRepository.Add(newAlias);

                    aliasId = newAlias.Id;
                    autoCreatedAliasThisRequest = true;

                    action = EmailProcessingAction.Forward;
                    forwardTo = mailbox.EmailAddress;
                }
                else
                {
                    action = EmailProcessingAction.Drop;

                    if (mailbox.AutoCreateAlias == false)
                    {
                        dropReason = DropReason.AutoCreateOff;
                    }
                    else
                    {
                        dropReason = DropReason.Policy;

                        activity?.SetTag("email.alias_autocreate.current_count", currentAliasCount);
                        activity?.SetTag("email.alias_autocreate.max_per_mailbox", maxAliasesPerMailbox?.ToString() ?? "unlimited");

                        activity?.SetTag("email.bandwidth.monthly.current_bytes", currentBandwidthThisMonth);
                        activity?.SetTag("email.bandwidth.monthly.max_bytes", maxBandwidthBytesPerPeriod?.ToString() ?? "unlimited");
                        activity?.SetTag("email.bandwidth.monthly.projected_bytes", projectedBandwidth);
                    }
                }
            }
            else
            {
                // Alias known
                aliasId = aliasMap.AliasId;
                mailboxId = aliasMap.MailboxId;
                accountId = aliasMap.AccountId;

                if (aliasMap.IsEnabled)
                {
                    action = EmailProcessingAction.Forward;
                    forwardTo = aliasMap.EmailAddress;
                }
                else
                {
                    action = EmailProcessingAction.Drop;
                    dropReason = DropReason.AliasDisabled;
                }
            }

            // Compute forward-from only if forwarding is still on
            if (action == EmailProcessingAction.Forward)
            {
                // Keep your existing pattern; consider replacing this with a config-based mg domain later.
                forwardFrom = $"{aliasLocal}+{routingKey}@{domain.Replace($"{routingKey}", "mg")}".ToLowerInvariant();
            }

            // ------------------------------------------------------------
            // DirectPassthrough + LearningMode + Sender rule
            // ------------------------------------------------------------
            var directPassthrough = aliasMap?.DirectPassthrough == true;

            // IMPORTANT:
            // If the alias was auto-created in this request, treat it like learning mode for the first delivery:
            // - allow the first sender
            // - write a learned rule for that sender
            // - track the sender
            var learningMode = autoCreatedAliasThisRequest || (aliasMap?.LearningMode == true);

            if (action == EmailProcessingAction.Forward &&
                aliasId is Guid a && a != Guid.Empty &&
                !string.IsNullOrWhiteSpace(senderEmailNormalized))
            {
                var finalDecisionForTracking = (DeliveryDecision?)DeliveryDecision.Forward;

                if (!directPassthrough)
                {
                    var ruleId = RuleId(senderEmailNormalized);
                    var senderRule = await _aliasRuleRepository.FindByIdAsync(ruleId, cancellationToken: cancellationToken);

                    if (senderRule is not null && senderRule.IsEnabled)
                    {
                        action = MapDecisionToAction(senderRule.Decision);

                        if (action == EmailProcessingAction.Drop)
                            dropReason = DropReason.Policy;

                        if (action == EmailProcessingAction.Quarantine)
                            quarantineReason = QuarantineReason.Policy;
                    }
                    else
                    {
                        // IMPORTANT:
                        // - disabled rule behaves like "no rule" but DOES NOT create/overwrite a rule.
                        // - write learned rules if LearningMode is ON OR this alias was auto-created this request.
                        var shouldWriteLearnedRule = (senderRule is null) && (learningMode || autoCreatedAliasThisRequest);

                        DeliveryDecision heuristicDecision;

                        if (learningMode || autoCreatedAliasThisRequest)
                        {
                            // Learning / first-delivery after auto-create: default allow.
                            heuristicDecision = DeliveryDecision.Forward;
                        }
                        else
                        {
                            // Learning mode OFF: default deny for unknown senders.
                            heuristicDecision = DeliveryDecision.Quarantine;
                        }

                        action = MapDecisionToAction(heuristicDecision);

                        if (action == EmailProcessingAction.Quarantine)
                        {
                            if (learningMode || autoCreatedAliasThisRequest)
                            {
                                quarantineReason = QuarantineReason.Policy;
                            }
                            else
                            {
                                // Distinguish "unknown sender" vs "suspicious sender" while still default-denying.
                                var suspicious = IsSuspiciousSender(senderEmailNormalized, senderDomainNormalized);
                                quarantineReason = suspicious
                                    ? QuarantineReason.SuspiciousSender
                                    : QuarantineReason.UnknownSender;
                            }
                        }

                        if (action == EmailProcessingAction.Drop)
                            dropReason = DropReason.Policy;

                        if (shouldWriteLearnedRule)
                        {
                            var newRule = new AliasRule(
                                id: ruleId,
                                aliasId: a,
                                ruleKind: AliasRuleKind.ExactEmail,
                                domain: string.Empty,
                                host: string.Empty,
                                email: senderEmailNormalized,
                                decision: heuristicDecision,
                                isEnabled: true,
                                source: AliasRuleSource.Learned);

                            _aliasRuleRepository.Add(newRule);
                        }
                    }

                    var finalDecision = action switch
                    {
                        EmailProcessingAction.Forward => DeliveryDecision.Forward,
                        EmailProcessingAction.Drop => DeliveryDecision.Drop,
                        EmailProcessingAction.Quarantine => DeliveryDecision.Quarantine,
                        _ => (DeliveryDecision?)null
                    };

                    finalDecisionForTracking = finalDecision;

                    if (action != EmailProcessingAction.Forward)
                    {
                        forwardTo = null;
                        forwardFrom = null;
                    }
                }

                // Track sender regardless (also for passthrough)
                var senderId = SenderId(senderEmailNormalized);

                var senderEntity = await _aliasSenderRepository.FindByIdAsync(senderId, cancellationToken: cancellationToken);
                if (senderEntity == null)
                {
                    senderEntity = new AliasSender(
                        id: senderId,
                        aliasId: a,
                        email: senderEmailNormalized,
                        domain: senderDomainNormalized,
                        lastDecision: finalDecisionForTracking);

                    _aliasSenderRepository.Add(senderEntity);
                }
                else
                {
                    senderEntity.RecordSeen(finalDecisionForTracking, receivedAtUtc);
                }

                // Optional: ensure rule exists for UI in passthrough, but do not overwrite existing/disabled rules.
                if (directPassthrough)
                {
                    var rid = RuleId(senderEmailNormalized);
                    var existingRule = await _aliasRuleRepository.FindByIdAsync(rid, cancellationToken: cancellationToken);
                    if (existingRule == null)
                    {
                        var passthroughRule = new AliasRule(
                            id: rid,
                            aliasId: a,
                            ruleKind: AliasRuleKind.ExactEmail,
                            domain: string.Empty,
                            host: string.Empty,
                            email: senderEmailNormalized,
                            decision: DeliveryDecision.Forward,
                            isEnabled: true,
                            source: AliasRuleSource.Learned);

                        _aliasRuleRepository.Add(passthroughRule);
                    }
                }
            }

            // ------------------------------------------------------------
            // NEW: Bandwidth gate must apply to ANY message that will be forwarded
            // (not just alias auto-create).
            // ------------------------------------------------------------
            if (action == EmailProcessingAction.Forward && accountId is Guid acc && acc != Guid.Empty)
            {
                var currentBandwidthThisMonth = await GetCurrentBandwidthForAccountMonthAsync(acc, receivedAtUtc, cancellationToken);
                var maxBandwidthBytesPerPeriod = await GetMaxBandwidthBytesPerPeriodAsync(acc, cancellationToken);

                bandwidthCurrentBytes = currentBandwidthThisMonth;
                bandwidthMaxBytes = maxBandwidthBytesPerPeriod;

                var projectedBandwidth = checked(currentBandwidthThisMonth + (long)size);
                bandwidthProjectedBytes = projectedBandwidth;

                var underBandwidthLimit = maxBandwidthBytesPerPeriod is null || projectedBandwidth <= maxBandwidthBytesPerPeriod.Value;

                activity?.SetTag("email.bandwidth.monthly.current_bytes", currentBandwidthThisMonth);
                activity?.SetTag("email.bandwidth.monthly.max_bytes", maxBandwidthBytesPerPeriod?.ToString() ?? "unlimited");
                activity?.SetTag("email.bandwidth.monthly.projected_bytes", projectedBandwidth);

                if (!underBandwidthLimit)
                {
                    bandwidthGateBlocked = true;

                    action = EmailProcessingAction.Drop;
                    dropReason = DropReason.Policy;

                    forwardTo = null;
                    forwardFrom = null;
                }
            }

            // ------------------------------------------------------------
            // ALWAYS log: build delivery action after final decision
            // ------------------------------------------------------------
            var partitionKey = BuildPartitionKey(aliasId, mailboxId, domain, receivedAtUtc);

            using var _ = _partitionKeyScope.Push(partitionKey);

            var deliveryAction = new DeliveryAction(
                partitionKey: partitionKey,
                aliasId: aliasId,
                mailboxId: mailboxId,
                accountId: accountId,
                source: source,
                receivedAt: receivedAtUtc,
                alias: aliasLocal,
                routingKey: routingKey,
                domain: domain,
                senderDisplay: senderDisplay,
                senderDomain: senderDomain,
                recipientDisplay: recipientDisplay,
                recipientDomain: recipientDomain,
                messageId: messageId,
                subject: subject,
                subjectHash: subjectHash,
                hasAttachments: hasAttachments,
                attachmentsCount: attachmentsCount,
                size: size
            );

            switch (action)
            {
                case EmailProcessingAction.Forward:
                    deliveryAction.Forward(forwardTo, forwardFrom);
                    break;

                case EmailProcessingAction.Drop:
                    {
                        var reason = dropReason ?? DropReason.Unknown;
                        string? msg = null;

                        if (reason == DropReason.AutoCreateOff)
                        {
                            msg = "Auto-create disabled.";
                        }
                        else if (reason == DropReason.Policy && bandwidthGateBlocked)
                        {
                            msg = "Bandwidth limit reached.";
                        }
                        else if (reason == DropReason.Policy && aliasMap == null && mailbox != null && mailbox.AutoCreateAlias)
                        {
                            // Note: this path can be alias-limit OR bandwidth-limit; bandwidth-limit already handled above.
                            msg = "Alias limit reached.";
                        }

                        dropReason = reason;
                        deliveryAction.Drop(reason, msg);
                        break;
                    }

                case EmailProcessingAction.Quarantine:
                    quarantineReason ??= QuarantineReason.Policy;
                    deliveryAction.Quarantine(quarantineReason.Value, null);
                    break;
            }

            _deliveryActionRepository.Add(deliveryAction);

            // Outcome telemetry/logging
            activity?.SetTag("email.action", action.ToString());

            switch (action)
            {
                case EmailProcessingAction.Forward:
                    activity?.SetTag("email.action", "forward");

                    ForwardedMessages.Add(1,
                        new("source", source),
                        new("domain", domain));

                    _logger.LogInformation(
                        "ProcessEmail decision: {Action} Domain={Domain} Source={Source} DeliveryActionId={DeliveryActionId}",
                        "Forward", domain, source, deliveryAction.Id);
                    break;

                case EmailProcessingAction.Drop:
                    activity?.SetTag("email.action", "drop");
                    activity?.SetTag("email.drop_reason", dropReason?.ToString());

                    DroppedMessages.Add(1,
                        new("source", source),
                        new("domain", domain),
                        new("reason", dropReason?.ToString() ?? "unknown"));

                    _logger.LogWarning(
                        "ProcessEmail decision: {Action} Reason={Reason} Domain={Domain} Source={Source} DeliveryActionId={DeliveryActionId}",
                        "Drop", dropReason, domain, source, deliveryAction.Id);
                    break;

                case EmailProcessingAction.Quarantine:
                    activity?.SetTag("email.action", "quarantine");
                    activity?.SetTag("email.quarantine_reason", quarantineReason?.ToString());

                    QuarantinedMessages.Add(1,
                        new("source", source),
                        new("domain", domain),
                        new("reason", quarantineReason?.ToString() ?? "unknown"));

                    _logger.LogWarning(
                        "ProcessEmail decision: {Action} Reason={Reason} Domain={Domain} Source={Source} DeliveryActionId={DeliveryActionId}",
                        "Quarantine", quarantineReason, domain, source, deliveryAction.Id);
                    break;
            }

            return new DeliveryDecisionDto
            {
                DeliveryActionId = deliveryAction.Id,
                PartitionKey = deliveryAction.PartitionKey,
                Action = action,
                ForwardTo = forwardTo,
                ForwardFrom = forwardFrom
            };
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            activity?.SetTag("exception.type", ex.GetType().FullName);
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("exception.stacktrace", ex.ToString());

            _logger.LogError(ex,
                "ProcessEmail failed. Domain={Domain} Source={Source}",
                domain, source);

            throw;
        }
        finally
        {
            sw.Stop();

            ProcessingDurationMs.Record(sw.Elapsed.TotalMilliseconds,
                new("source", source),
                new("domain", domain));

            activity?.SetTag("email.duration_ms", sw.Elapsed.TotalMilliseconds);
        }
    }
}
