using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.UpdateAlias;

public class UpdateAliasCommandHandler : IRequestHandler<UpdateAliasCommand>
{
    private readonly IAliasRepository _aliasRepository;
    private readonly IAliasMapRepository _aliasMapRepository;
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAliasCommandHandler(
        IAliasRepository aliasRepository,
        IAliasMapRepository aliasMapRepository,
        IMailboxRepository mailboxRepository,
        IAccountUserMapRepository accountUserMapRepository,
        ICurrentUserService currentUserService)
    {
        _aliasRepository = aliasRepository;
        _aliasMapRepository = aliasMapRepository;
        _mailboxRepository = mailboxRepository;
        _accountUserMapRepository = accountUserMapRepository;
        _currentUserService = currentUserService;
    }

    [IntentIgnore]
    public async Task Handle(UpdateAliasCommand request, CancellationToken cancellationToken)
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

        var mailbox = await _mailboxRepository.FindAsync(m => m.Id == request.MailboxId && m.AccountId == account.Id, cancellationToken);
        if (mailbox is null)
        {
            throw new NotFoundException($"Could not find Mailbox '{request.MailboxId}'");
        }

        var alias = await _aliasRepository.FindAsync(a => a.Id == request.Id && a.MailboxId == request.MailboxId, cancellationToken);
        if (alias is null)
        {
            throw new NotFoundException($"Could not find Alias '{request.Id}'");
        }

        alias.Update(request.Name, request.IsEnabled, request.DirectPassthrough, request.LearningMode);
    }
}