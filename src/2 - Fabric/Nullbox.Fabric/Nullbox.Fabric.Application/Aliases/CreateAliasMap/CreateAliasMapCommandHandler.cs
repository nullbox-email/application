using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Entities.Aliases;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.CreateAliasMap;

public class CreateAliasMapCommandHandler : IRequestHandler<CreateAliasMapCommand>
{
    private readonly IAliasRepository _aliasRepository;
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IAliasMapRepository _aliasMapRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public CreateAliasMapCommandHandler(
        IAliasRepository aliasRepository,
        IMailboxRepository mailboxRepository,
        IAliasMapRepository aliasMapRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _aliasRepository = aliasRepository;
        _mailboxRepository = mailboxRepository;
        _aliasMapRepository = aliasMapRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    [IntentIgnore]
    public async Task Handle(CreateAliasMapCommand request, CancellationToken cancellationToken)
    {
        var alias = await _aliasRepository.FindAsync(a => a.Id == request.Id && a.MailboxId == request.MailboxId, cancellationToken);
        var mailbox = await _mailboxRepository.FindAsync(m => m.Id == request.MailboxId && m.AccountId == request.AccountId, cancellationToken);

        var _alias = alias.LocalPart.Trim().ToLowerInvariant();
        var _routingKey = mailbox.RoutingKey.Trim().ToLowerInvariant();
        var _domain = mailbox.Domain.Trim().ToLowerInvariant();

        var fullyQualifiedAlias = $"{_alias}@{_routingKey}.{_domain}";
        
        using var _ = _partitionKeyScope.Push(fullyQualifiedAlias);

        var aliasMap = new AliasMap(
            id: fullyQualifiedAlias,
            accountId: request.AccountId,
            mailboxId: request.MailboxId,
            aliasId: request.Id,
            isEnabled: request.IsEnabled,
            directPassthrough: request.DirectPassthrough,
            learningMode: request.LearningMode,
            autoCreateAlias: mailbox.AutoCreateAlias,
            emailAddress: mailbox.EmailAddress);

        _aliasMapRepository.Add(aliasMap);
    }
}