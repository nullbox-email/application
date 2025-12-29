using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.GetUserMailboxes;

public class GetUserMailboxesQueryHandler : IRequestHandler<GetUserMailboxesQuery, List<MailboxDto>>
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IAliasRepository _aliasRepository;
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUserMailboxesQueryHandler(
        IMailboxRepository mailboxRepository,
        IAliasRepository aliasRepository,
        IAccountUserMapRepository accountUserMapRepository,
        ICurrentUserService currentUserService)
    {
        _mailboxRepository = mailboxRepository;
        _aliasRepository = aliasRepository;
        _accountUserMapRepository = accountUserMapRepository;
        _currentUserService = currentUserService;
    }

    [IntentIgnore]
    public async Task<List<MailboxDto>> Handle(
            GetUserMailboxesQuery request,
            CancellationToken cancellationToken)
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

        var mailboxes = await _mailboxRepository.FindAllAsync(m => m.AccountId == account.Id, cancellationToken);
        var mailboxIds = mailboxes.Select(m => m.Id).ToList();

        var aliases = await _aliasRepository.FindAllAsync(a => mailboxIds.Contains(a.MailboxId), cancellationToken);

        var mailboxDtos = mailboxes.Select(m =>
        {
            var mailboxAliases = aliases
                .Where(a => a.MailboxId == m.Id)
                .Select(a => AliasDto.Create(
                    a.Id, 
                    a.MailboxId, 
                    a.Name, 
                    a.LocalPart, 
                    a.IsEnabled,
                    a.DirectPassthrough,
                    a.LearningMode))
                .ToList();

            return MailboxDto.Create(
                m.Id, 
                m.Name, 
                m.RoutingKey, 
                m.Domain, 
                mailboxAliases, 
                m.AutoCreateAlias,
                m.EmailAddress);
        }).ToList();

        return mailboxDtos;
    }
}