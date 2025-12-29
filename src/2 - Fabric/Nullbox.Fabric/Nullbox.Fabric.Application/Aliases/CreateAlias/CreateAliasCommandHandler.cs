using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Entities.Aliases;
using Nullbox.Fabric.Domain.Markers;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Aliases;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Statistics;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Aliases.CreateAlias;

public class CreateAliasCommandHandler : IRequestHandler<CreateAliasCommand, string>
{
    private readonly IAliasRepository _aliasRepository;
    private readonly IAliasMapRepository _aliasMapRepository;
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly ICurrentUserService _currentUserService;

    private readonly IEffectiveEnablementRepository _effectiveEnablementRepository;
    private readonly ITrafficStatisticRepository _trafficStatisticRepository;

    public CreateAliasCommandHandler(
        IAliasRepository aliasRepository,
        IAliasMapRepository aliasMapRepository,
        IMailboxRepository mailboxRepository,
        IAccountUserMapRepository accountUserMapRepository,
        ICurrentUserService currentUserService,
        IEffectiveEnablementRepository effectiveEnablementRepository,
        ITrafficStatisticRepository trafficStatisticRepository)
    {
        _aliasRepository = aliasRepository;
        _aliasMapRepository = aliasMapRepository;
        _mailboxRepository = mailboxRepository;
        _accountUserMapRepository = accountUserMapRepository;
        _currentUserService = currentUserService;

        _effectiveEnablementRepository = effectiveEnablementRepository;
        _trafficStatisticRepository = trafficStatisticRepository;
    }

    // Matches ProcessStatistics.Keys: BucketKey(MarkerType.Mailbox, "a:all") => $"t:{markerType}|{timeBucketKey}"
    private static string AllTimeMailboxBucketKey() => $"t:{MarkerType.Mailbox}|a:all";

    private async Task<int> GetCurrentAliasCountForMailboxAsync(Guid mailboxId, CancellationToken cancellationToken)
    {
        var statId = AllTimeMailboxBucketKey();

        var stat = await _trafficStatisticRepository.FindAsync(
            s => s.Id == statId && s.PartitionKey == mailboxId,
            cancellationToken);

        return stat?.Aliases ?? 0;
    }

    private async Task<int?> GetMaxAliasesPerMailboxAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var effective = await _effectiveEnablementRepository.FindAsync(
            e => e.Id == accountId && e.AccountId == accountId,
            cancellationToken);

        // null => unlimited
        return effective?.MaxAliasesPerMailbox;
    }

    [IntentIgnore]
    public async Task<string> Handle(CreateAliasCommand request, CancellationToken cancellationToken)
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

        var mailbox = await _mailboxRepository.FindAsync(
            m => m.Id == request.MailboxId && m.AccountId == account.Id,
            cancellationToken);

        if (mailbox is null)
        {
            throw new NotFoundException($"Could not find Mailbox '{request.MailboxId}'");
        }

        // Enforce alias-per-mailbox limit (null => unlimited)
        var currentAliasCount = await GetCurrentAliasCountForMailboxAsync(mailbox.Id, cancellationToken);
        var maxAliasesPerMailbox = await GetMaxAliasesPerMailboxAsync(account.Id, cancellationToken);

        if (maxAliasesPerMailbox is not null && currentAliasCount >= maxAliasesPerMailbox.Value)
        {
            throw new LimitReachedException("Alias limit reached.");
        }

        var _alias = request.LocalPart.Trim().ToLowerInvariant();
        var _routingKey = mailbox.RoutingKey.Trim().ToLowerInvariant();
        var _domain = mailbox.Domain.Trim().ToLowerInvariant();

        var fullyQualifiedAlias = $"{_alias}@{_routingKey}.{_domain}";

        var existingAliasMap = await _aliasMapRepository.FindAsync(a => a.Id == fullyQualifiedAlias, cancellationToken);
        if (existingAliasMap is not null)
        {
            throw new ConflictException($"An alias with the name '{fullyQualifiedAlias}' already exists.");
        }

        var alias = new Alias(
            mailboxId: request.MailboxId,
            accountId: account.Id,
            name: request.Name,
            localPart: request.LocalPart);

        _aliasRepository.Add(alias);

        return alias.LocalPart;
    }
}
