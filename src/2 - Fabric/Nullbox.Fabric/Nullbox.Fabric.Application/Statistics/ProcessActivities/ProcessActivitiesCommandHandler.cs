using System.Globalization;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Domain.Entities.Activities;
using Nullbox.Fabric.Domain.Entities.Markers;
using Nullbox.Fabric.Domain.Markers;
using Nullbox.Fabric.Domain.Repositories.Activities;
using Nullbox.Fabric.Domain.Repositories.Markers;
using Nullbox.Fabric.Domain.ValueObjects;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Statistics.ProcessActivities;

public class ProcessActivitiesCommandHandler : IRequestHandler<ProcessActivitiesCommand>
{
    private readonly IRecentDeliveryActionRepository _recentDeliveryActionRepository;
    private readonly IAppliedMarkerRepository _appliedMarkerRepository;

    public ProcessActivitiesCommandHandler(
        IRecentDeliveryActionRepository recentDeliveryActionRepository,
        IAppliedMarkerRepository appliedMarkerRepository)
    {
        _recentDeliveryActionRepository = recentDeliveryActionRepository;
        _appliedMarkerRepository = appliedMarkerRepository;
    }

    public async Task Handle(ProcessActivitiesCommand request, CancellationToken cancellationToken)
    {
        var receivedAtUtc = request.ReceivedAt.ToUniversalTime();
        var sortKey = Keys.RecentSortKey(receivedAtUtc);
        var rowId = $"{sortKey}|da:{request.Id:D}";

        if (request.AliasId is Guid aliasId)
        {
            await TryApplyRecentDeliveryActionAsync(
                request: request,
                markerType: MarkerType.Alias,
                scopePartitionGuid: aliasId,
                projectionId: Keys.ActivityProjectionId(MarkerType.Alias, rowId),
                sortKey: sortKey,
                receivedAtUtc: receivedAtUtc,
                requireMailboxAndAlias: false,
                cancellationToken: cancellationToken);
        }

        if (request.MailboxId is Guid mailboxId)
        {
            await TryApplyRecentDeliveryActionAsync(
                request: request,
                markerType: MarkerType.Mailbox,
                scopePartitionGuid: mailboxId,
                projectionId: Keys.ActivityProjectionId(MarkerType.Mailbox, rowId),
                sortKey: sortKey,
                receivedAtUtc: receivedAtUtc,
                requireMailboxAndAlias: false,
                cancellationToken: cancellationToken);
        }

        if (request.AccountId is Guid accountId)
        {
            await TryApplyRecentDeliveryActionAsync(
                request: request,
                markerType: MarkerType.Account,
                scopePartitionGuid: accountId,
                projectionId: Keys.ActivityProjectionId(MarkerType.Account, rowId),
                sortKey: sortKey,
                receivedAtUtc: receivedAtUtc,
                requireMailboxAndAlias: true,
                cancellationToken: cancellationToken);
        }

        // No SaveChanges for projections here; pipeline commits once.
        // Marker commits are immediate (gate).
    }

    private async Task TryApplyRecentDeliveryActionAsync(
        ProcessActivitiesCommand request,
        MarkerType markerType,
        Guid scopePartitionGuid,
        string projectionId,
        string sortKey,
        DateTimeOffset receivedAtUtc,
        bool requireMailboxAndAlias,
        CancellationToken cancellationToken)
    {
        if (requireMailboxAndAlias && (request.MailboxId is not Guid || request.AliasId is not Guid))
        {
            return;
        }

        var markerPartitionKey = Keys.MarkerPartitionKey(markerType, scopePartitionGuid);
        var markerId = Keys.MarkerId(markerType, MarkerGroup.Activities, MarkerStage.Created, projectionId, request.Id);

        var created = await EnsureMarkerAsync(
            markerId: markerId,
            markerPartitionKey: markerPartitionKey,
            group: MarkerGroup.Activities,
            stage: MarkerStage.Created,
            key: projectionId,
            deliveryActionId: request.Id,
            cancellationToken: cancellationToken);

        if (!created)
        {
            return;
        }

        await UpsertRecentDeliveryActionAsync(
            request: request,
            partitionKey: scopePartitionGuid,
            id: projectionId,
            sortKey: sortKey,
            receivedAtUtc: receivedAtUtc,
            cancellationToken: cancellationToken);
    }

    private async Task UpsertRecentDeliveryActionAsync(
        ProcessActivitiesCommand request,
        Guid partitionKey,
        string id,
        string sortKey,
        DateTimeOffset receivedAtUtc,
        CancellationToken cancellationToken)
    {
        var existing = await _recentDeliveryActionRepository.FindAsync(
            x => x.Id == id && x.PartitionKey == partitionKey,
            cancellationToken);

        var dropReason = request.DropReason is not null
            ? new DropReasonValueObject(request.DropReason.Reason, request.DropReason.Message)
            : null;

        var quarantineReason = request.QuarantineReason is not null
            ? new QuarantineReasonValueObject(request.QuarantineReason.Reason, request.QuarantineReason.Message)
            : null;

        var now = DateTimeOffset.UtcNow;

        if (existing is null)
        {
            var entity = new RecentDeliveryAction(
                id: id,
                partitionKey: partitionKey,
                deliveryActionId: request.Id,
                receivedAt: receivedAtUtc,
                sortKey: sortKey,
                senderDisplay: request.SenderDisplay,
                senderDomain: request.SenderDomain,
                recipientDisplay: request.RecipientDisplay,
                recipientDomain: request.RecipientDomain,
                subject: request.Subject,
                subjectHash: request.SubjectHash,
                hasAttachments: request.HasAttachments,
                attachmentsCount: request.AttachmentsCount,
                size: request.Size,
                deliveryDecision: request.DeliveryDecision,
                dropReason: dropReason,
                quarantineReason: quarantineReason,
                forwardTo: request.ForwardTo,
                forwardFrom: request.ForwardFrom,
                provider: request.Provider,
                providerStatus: request.ProviderStatus,
                providerMessageId: request.ProviderMessageId,
                providerError: request.ProviderError,
                completedAt: request.CompletedAt,
                updatedAt: now);

            _recentDeliveryActionRepository.Add(entity);
            return;
        }

        existing.Update(
            senderDisplay: request.SenderDisplay,
            senderDomain: request.SenderDomain,
            recipientDisplay: request.RecipientDisplay,
            recipientDomain: request.RecipientDomain,
            subject: request.Subject,
            subjectHash: request.SubjectHash,
            hasAttachments: request.HasAttachments,
            attachmentsCount: request.AttachmentsCount,
            size: request.Size,
            deliveryDecision: request.DeliveryDecision,
            dropReason: dropReason,
            quarantineReason: quarantineReason,
            forwardTo: request.ForwardTo,
            forwardFrom: request.ForwardFrom,
            provider: request.Provider,
            providerStatus: request.ProviderStatus,
            providerMessageId: request.ProviderMessageId,
            providerError: request.ProviderError,
            completedAt: request.CompletedAt,
            updatedAt: now);

        _recentDeliveryActionRepository.Update(existing);
    }

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

        // Commit marker immediately as the gate
        await _appliedMarkerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static class Keys
    {
        public static string RecentSortKey(DateTimeOffset receivedAtUtc)
        {
            var inv = DateTime.MaxValue.Ticks - receivedAtUtc.UtcDateTime.Ticks;
            return "rt:" + inv.ToString("D19", CultureInfo.InvariantCulture);
        }

        public static string ActivityProjectionId(MarkerType markerType, string rowId) =>
            $"t:{markerType}|{rowId}";

        public static string MarkerPartitionKey(MarkerType markerType, Guid scopePartitionGuid) =>
            markerType == MarkerType.System ? "system" : scopePartitionGuid.ToString();

        public static string MarkerId(MarkerType type, MarkerGroup group, MarkerStage stage, string key, Guid deliveryActionId) =>
            $"t:{type}|g:{group}|k:{key}|s:{stage}|da:{deliveryActionId:D}";
    }
}
