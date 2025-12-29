using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.MailboxMapUpdateMailbox;

public class MailboxMapUpdateMailboxCommandHandler : IRequestHandler<MailboxMapUpdateMailboxCommand>
{
    private readonly IMailboxMapRepository _mailboxMapRepository;

    public MailboxMapUpdateMailboxCommandHandler(
        IMailboxMapRepository mailboxMapRepository)
    {
        _mailboxMapRepository = mailboxMapRepository;
    }

    [IntentIgnore]
    public async Task Handle(MailboxMapUpdateMailboxCommand request, CancellationToken cancellationToken)
    {
        var mailboxMap = await _mailboxMapRepository.FindByIdAsync($"{request.RoutingKey}.{request.Domain}", cancellationToken);
        if (mailboxMap is null)
        {
            return;
        }

        mailboxMap.Update(request.AutoCreateAlias, request.EmailAddress);
    }
}