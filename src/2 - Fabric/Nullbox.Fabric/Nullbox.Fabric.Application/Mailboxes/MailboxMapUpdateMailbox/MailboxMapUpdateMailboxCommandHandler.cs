using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.MailboxMapUpdateMailbox;

public class MailboxMapUpdateMailboxCommandHandler : IRequestHandler<MailboxMapUpdateMailboxCommand>
{
    private readonly IMailboxMapRepository _mailboxMapRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public MailboxMapUpdateMailboxCommandHandler(
        IMailboxMapRepository mailboxMapRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _mailboxMapRepository = mailboxMapRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    [IntentIgnore]
    public async Task Handle(MailboxMapUpdateMailboxCommand request, CancellationToken cancellationToken)
    {
        var mailboxMapId = $"{request.RoutingKey}.{request.Domain}";
        using var _ = _partitionKeyScope.Push(mailboxMapId);

        var mailboxMap = await _mailboxMapRepository.FindByIdAsync(mailboxMapId, cancellationToken);
        if (mailboxMap is null)
        {
            return;
        }

        mailboxMap.Update(request.AutoCreateAlias, request.EmailAddress);
    }
}