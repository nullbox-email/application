using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.AliasMapUpdateMailbox;

public class AliasMapUpdateMailboxCommandHandler : IRequestHandler<AliasMapUpdateMailboxCommand>
{
    private readonly IAliasRepository _aliasRepository;
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IAliasMapRepository _aliasMapRepository;

    public AliasMapUpdateMailboxCommandHandler(
        IAliasRepository aliasRepository,
        IMailboxRepository mailboxRepository,
        IAliasMapRepository aliasMapRepository)
    {
        _aliasRepository = aliasRepository;
        _mailboxRepository = mailboxRepository;
        _aliasMapRepository = aliasMapRepository;
    }

    [IntentIgnore]
    public async Task Handle(AliasMapUpdateMailboxCommand request, CancellationToken cancellationToken)
    {
        var mailboxes = await _mailboxRepository.FindAllAsync(m => m.AccountId == request.AccountId, cancellationToken);
        var mailboxIds = mailboxes.Select(m => m.Id).ToList();

        var aliases = await _aliasRepository.FindAllAsync(a => mailboxIds.Contains(a.MailboxId), cancellationToken);

        foreach (var mailbox in mailboxes)
        {
            foreach (var alias in aliases)
            {
                var _alias = alias.LocalPart.Trim().ToLowerInvariant();
                var _routingKey = mailbox.RoutingKey.Trim().ToLowerInvariant();
                var _domain = mailbox.Domain.Trim().ToLowerInvariant();

                var aliasMapFullyQualifiedName = $"{_alias}@{_routingKey}.{_domain}";

                var aliasMap = await _aliasMapRepository.FindByIdAsync(aliasMapFullyQualifiedName, cancellationToken);
                if (aliasMap is null)
                {
                    continue;
                }

                aliasMap.UpdateMailbox(mailbox.AutoCreateAlias, mailbox.EmailAddress);
            }
        }
    }
}