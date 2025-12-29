using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.GetUserMailboxByRoutingKeyAndDomain;

public class GetUserMailboxByRoutingKeyAndDomainQueryHandler : IRequestHandler<GetUserMailboxByRoutingKeyAndDomainQuery, UserMailboxDto>
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IMailboxMapRepository _mailboxMapRepository;
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetUserMailboxByRoutingKeyAndDomainQueryHandler(
        IMailboxRepository mailboxRepository,
        IMailboxMapRepository mailboxMapRepository,
        IAccountUserMapRepository accountUserMapRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _mailboxRepository = mailboxRepository;
        _mailboxMapRepository = mailboxMapRepository;
        _accountUserMapRepository = accountUserMapRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    [IntentIgnore]
    public async Task<UserMailboxDto> Handle(
        GetUserMailboxByRoutingKeyAndDomainQuery request,
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

        var normalizedMailboxId = request.Id.Trim().ToLowerInvariant();

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

        return mailbox.MapToUserMailboxDto(_mapper);
    }
}