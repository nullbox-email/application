using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateMailboxMap;

public class CreateMailboxMapCommandHandler : IRequestHandler<CreateMailboxMapCommand>
{
    private readonly IMailboxMapRepository _mailboxMapRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public CreateMailboxMapCommandHandler(
        IMailboxMapRepository mailboxMapRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _mailboxMapRepository = mailboxMapRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    public async Task Handle(CreateMailboxMapCommand request, CancellationToken cancellationToken)
    {
        using var _ = _partitionKeyScope.Push(request.Id.ToString());

        var mailboxMap = new MailboxMap(
            id: request.Id,
            mailboxId: request.MailboxId,
            accountId: request.UserId,
            autoCreateAlias: request.AutoCreateAlias,
            emailAddress: request.EmailAddress);

        _mailboxMapRepository.Add(mailboxMap);
    }
}