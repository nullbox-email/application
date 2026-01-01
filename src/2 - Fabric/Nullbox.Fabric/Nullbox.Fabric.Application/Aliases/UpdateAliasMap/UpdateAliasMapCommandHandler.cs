using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.UpdateAliasMap;

public class UpdateAliasMapCommandHandler : IRequestHandler<UpdateAliasMapCommand>
{
    private readonly IAliasRepository _aliasRepository;
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IAliasMapRepository _aliasMapRepository;
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public UpdateAliasMapCommandHandler(
        IAliasRepository aliasRepository,
        IMailboxRepository mailboxRepository, 
        IAliasMapRepository aliasMapRepository,
        IAccountUserMapRepository accountUserMapRepository,
        ICurrentUserService currentUserService,
        IPartitionKeyScope partitionKeyScope)
    {
        _aliasRepository = aliasRepository;
        _mailboxRepository = mailboxRepository;
        _aliasMapRepository = aliasMapRepository;
        _accountUserMapRepository = accountUserMapRepository;
        _currentUserService = currentUserService;
        _partitionKeyScope = partitionKeyScope;
    }

    [IntentIgnore]
    public async Task Handle(UpdateAliasMapCommand request, CancellationToken cancellationToken)
    {
        var alias = await _aliasRepository.FindAsync(a => a.Id == request.AliasId && a.MailboxId == request.MailboxId, cancellationToken);
        var mailbox = await _mailboxRepository.FindAsync(m => m.Id == request.MailboxId && m.AccountId == request.AccountId, cancellationToken);

        var _alias = alias.LocalPart.Trim().ToLowerInvariant();
        var _routingKey = mailbox.RoutingKey.Trim().ToLowerInvariant();
        var _domain = mailbox.Domain.Trim().ToLowerInvariant();

        var fullyQualifiedAlias = $"{_alias}@{_routingKey}.{_domain}";

        using var _ = _partitionKeyScope.Push(fullyQualifiedAlias);

        var aliasMap = await _aliasMapRepository.FindAsync(x => x.Id == fullyQualifiedAlias && x.MailboxId == request.MailboxId, cancellationToken);
        if (aliasMap is null)
        {
            throw new NotFoundException($"Could not find AliasMap '({request.Id}, {request.MailboxId})'");
        }

        aliasMap.UpdateAlias(request.IsEnabled, request.DirectPassthrough, request.LearningMode);
    }
}