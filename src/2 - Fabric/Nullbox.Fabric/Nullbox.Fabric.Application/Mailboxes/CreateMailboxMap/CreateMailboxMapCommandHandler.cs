using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateMailboxMap;

public class CreateMailboxMapCommandHandler : IRequestHandler<CreateMailboxMapCommand>
{
    private readonly IMailboxMapRepository _mailboxMapRepository;

    public CreateMailboxMapCommandHandler(IMailboxMapRepository mailboxMapRepository)
    {
        _mailboxMapRepository = mailboxMapRepository;
    }

    public async Task Handle(CreateMailboxMapCommand request, CancellationToken cancellationToken)
    {
        var mailboxMap = new MailboxMap(
            id: request.Id,
            mailboxId: request.MailboxId,
            accountId: request.UserId,
            autoCreateAlias: request.AutoCreateAlias,
            emailAddress: request.EmailAddress);

        _mailboxMapRepository.Add(mailboxMap);
    }
}