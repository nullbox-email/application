using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Mailboxes;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.Aliases.GetAliasById;

public class GetAliasByIdQueryHandler : IRequestHandler<GetAliasByIdQuery, AliasDto>
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IMailboxMapRepository _mailboxMapRepository;
    private readonly IAliasRepository _aliasRepository;
    private readonly IAliasMapRepository _aliasMapRepository;
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAliasByIdQueryHandler(
        IMailboxRepository mailboxRepository,
        IMailboxMapRepository mailboxMapRepository,
        IAliasRepository aliasRepository,
        IAliasMapRepository aliasMapRepository,
        IAccountUserMapRepository accountUserMapRepository,
        ICurrentUserService currentUserService)
    {
        _mailboxRepository = mailboxRepository;
        _mailboxMapRepository = mailboxMapRepository;
        _aliasRepository = aliasRepository;
        _aliasMapRepository = aliasMapRepository;
        _accountUserMapRepository = accountUserMapRepository;
        _currentUserService = currentUserService;
    }

    [IntentIgnore]
    public async Task<AliasDto> Handle(GetAliasByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserService.GetAsync();

        if (currentUser is null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        if (!Guid.TryParse(currentUser.Id, out var userId))
        {
            throw new ForbiddenAccessException("Invalid user ID.");
        }

        var account = await _accountUserMapRepository.FindAsync(a => a.PartitionKey == userId, cancellationToken);
        if (account is null)
        {
            throw new ForbiddenAccessException("User does not have access to any account.");
        }

        var normalizedMailboxId = request.MailboxId.Trim().ToLowerInvariant();

        var mailboxMap = await _mailboxMapRepository.FindAsync(m => m.Id == normalizedMailboxId && m.AccountId == account.Id, cancellationToken);
        if (mailboxMap is null)
        {
            throw new NotFoundException($"Mailbox '{normalizedMailboxId}' not found.");
        }

        var mailbox = await _mailboxRepository.FindAsync(m => m.Id == mailboxMap.MailboxId && m.AccountId == account.Id, cancellationToken);
        if (mailbox is null)
        {
            throw new NotFoundException($"Mailbox '{mailboxMap.MailboxId}' not found.");
        }

        var normalizedAliasId = $"{request.Id.Trim().ToLowerInvariant()}@{normalizedMailboxId}";

        var aliasMap = await _aliasMapRepository.FindAsync(a => a.Id == normalizedAliasId && a.MailboxId == mailbox.Id, cancellationToken);
        if (aliasMap is null)
        {
            throw new NotFoundException($"Alias '{normalizedAliasId}' not found for Mailbox '{mailbox.Id}'.");
        }

        var alias = await _aliasRepository.FindAsync(a => a.Id == aliasMap.AliasId && a.MailboxId == mailbox.Id, cancellationToken);
        if (alias is null)
        {
            throw new NotFoundException($"Alias '{aliasMap.AliasId}' not found for Mailbox '{mailbox.Id}'.");
        }

        return new AliasDto
        {
            Id = alias.Id,
            MailboxId = alias.MailboxId,
            Name = alias.Name,
            LocalPart = alias.LocalPart,
            IsEnabled = alias.IsEnabled,
            DirectPassthrough = alias.DirectPassthrough,
            LearningMode = alias.LearningMode
        };
    }
}