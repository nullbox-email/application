using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Application.Common.Storage;
using Nullbox.Fabric.Domain.Repositories.Deliveries;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Deliveries.QuarantineEmail;

public class QuarantineEmailCommandHandler : IRequestHandler<QuarantineEmailCommand>
{
    private readonly IDeliveryActionRepository _deliveryActionRepository;
    private readonly IBlobStorage _blobStorage;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public QuarantineEmailCommandHandler(
        IDeliveryActionRepository deliveryActionRepository,
        IBlobStorage blobStorage,
        IPartitionKeyScope partitionKeyScope)
    {
        _deliveryActionRepository = deliveryActionRepository;
        _blobStorage = blobStorage;
        _partitionKeyScope = partitionKeyScope;
    }

    public async Task Handle(QuarantineEmailCommand request, CancellationToken cancellationToken)
    {
        using var _ = _partitionKeyScope.Push(request.PartitionKey);

        var deliveryAction = await _deliveryActionRepository.FindAsync(da => da.Id == request.DeliveryActionId && da.PartitionKey == request.PartitionKey, cancellationToken);

        if (deliveryAction is null)
        {
            throw new InvalidOperationException("Delivery action not found.");
        }

        if (deliveryAction.AliasId == null)
        {
            throw new InvalidOperationException("Cannot quarantine email for delivery action without associated alias.");
        }

        await _blobStorage.UploadAsync(
            containerName: deliveryAction.MailboxId.Value.ToString(),
            blobName: $"{deliveryAction.AliasId}/{deliveryAction.Id}",
            dataStream: request.Content,
            cancellationToken: cancellationToken);

        deliveryAction.Quarantine(request.Reason, request.Message);

        _deliveryActionRepository.Update(deliveryAction);
    }
}