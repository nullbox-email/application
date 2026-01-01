using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateMailboxRoutingKeyMap;

public class CreateMailboxRoutingKeyMapCommandHandler : IRequestHandler<CreateMailboxRoutingKeyMapCommand>
{
    private readonly IMailboxRoutingKeyMapRepository _mailboxRoutingKeyMapRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public CreateMailboxRoutingKeyMapCommandHandler(
        IMailboxRoutingKeyMapRepository mailboxRoutingKeyMapRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _mailboxRoutingKeyMapRepository = mailboxRoutingKeyMapRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    public async Task Handle(CreateMailboxRoutingKeyMapCommand request, CancellationToken cancellationToken)
    {
        // [IntentIgnore]
        using var _ = _partitionKeyScope.Push(request.RoutingKey);

        var mailboxRoutingKeyMap = new MailboxRoutingKeyMap(
            id: request.RoutingKey,
            mailboxId: request.Id,
            accountId: request.UserId);

        _mailboxRoutingKeyMapRepository.Add(mailboxRoutingKeyMap);
    }
}