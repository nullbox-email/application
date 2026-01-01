using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.UpdateMailbox;

public class UpdateMailboxCommandHandler : IRequestHandler<UpdateMailboxCommand>
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public UpdateMailboxCommandHandler(
        IMailboxRepository mailboxRepository,
        IAccountUserMapRepository accountUserMapRepository,
        ICurrentUserService currentUserService,
        IPartitionKeyScope partitionKeyScope)
    {
        _mailboxRepository = mailboxRepository;
        _accountUserMapRepository = accountUserMapRepository;
        _currentUserService = currentUserService;
        _partitionKeyScope = partitionKeyScope;
    }

    [IntentIgnore]
    public async Task Handle(UpdateMailboxCommand request, CancellationToken cancellationToken)
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

        var mailbox = await _mailboxRepository.FindAsync(m => m.Id == request.Id && m.AccountId == account.Id, cancellationToken);
        if (mailbox is null)
        {
            throw new NotFoundException($"Could not find Mailbox '{request.Id}'");
        }

        using var _ = _partitionKeyScope.Push(account.Id.ToString());

        mailbox.Update(request.Name, request.AutoCreateAlias, request.EmailAddress);
    }
}