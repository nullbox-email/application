using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Common.Exceptions;
using Nullbox.Fabric.Domain.Entities.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Accounts;
using Nullbox.Fabric.Domain.Repositories.Mailboxes;
using Nullbox.Fabric.Domain.Repositories.Statistics;
using Nullbox.Fabric.Domain.Services.NumberGenerator;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateMailbox;

public class CreateMailboxCommandHandler : IRequestHandler<CreateMailboxCommand, string>
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IMailboxRoutingKeyMapRepository _routingKeyMapRepository;
    private readonly IUniqueIdentifierGenerator _uniqueIdentifierGenerator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly IEffectiveEnablementRepository _effectiveEnablementRepository;
    private readonly ITrafficStatisticRepository _trafficStatisticRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public CreateMailboxCommandHandler(
        IMailboxRepository mailboxRepository,
        IMailboxRoutingKeyMapRepository routingKeyMapRepository,
        IUniqueIdentifierGenerator uniqueIdentifierGenerator,
        ICurrentUserService currentUserService,
        IAccountRepository accountRepository,
        IAccountUserMapRepository accountUserMapRepository,
        IEffectiveEnablementRepository effectiveEnablementRepository,
        ITrafficStatisticRepository trafficStatisticRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _mailboxRepository = mailboxRepository;
        _routingKeyMapRepository = routingKeyMapRepository;
        _uniqueIdentifierGenerator = uniqueIdentifierGenerator;
        _currentUserService = currentUserService;
        _accountRepository = accountRepository;
        _accountUserMapRepository = accountUserMapRepository;
        _effectiveEnablementRepository = effectiveEnablementRepository;
        _trafficStatisticRepository = trafficStatisticRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    // Matches ProcessStatistics.Keys: BucketKey(MarkerType.Account, "a:all") => $"t:{markerType}|{timeBucketKey}"
    private static string AllTimeAccountBucketKey() => $"t:{Domain.Markers.MarkerType.Account}|a:all";

    private async Task<int> GetCurrentMailboxCountForAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var statId = AllTimeAccountBucketKey();

        var stat = await _trafficStatisticRepository.FindAsync(
            s => s.Id == statId && s.PartitionKey == accountId,
            cancellationToken);

        return stat?.Mailboxes ?? 0;
    }

    private async Task<int?> GetMaxMailboxesForAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var effective = await _effectiveEnablementRepository.FindAsync(
            e => e.Id == accountId && e.AccountId == accountId,
            cancellationToken);

        // null => unlimited
        return effective?.MaxMailboxes;
    }

    [IntentIgnore]
    public async Task<string> Handle(CreateMailboxCommand request, CancellationToken cancellationToken)
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

        var accountMap = await _accountUserMapRepository.FindAsync(a => a.PartitionKey == userId, cancellationToken);
        if (accountMap is null)
        {
            throw new ForbiddenAccessException("User does not have access to any account.");
        }

        var account = await _accountRepository.FindByIdAsync(accountMap.Id, cancellationToken);

        // Enforce mailbox limit (null => unlimited)
        var currentMailboxCount = await GetCurrentMailboxCountForAccountAsync(accountMap.Id, cancellationToken);
        var maxMailboxes = await GetMaxMailboxesForAccountAsync(accountMap.Id, cancellationToken);

        if (maxMailboxes is not null && currentMailboxCount >= maxMailboxes.Value)
        {
            throw new LimitReachedException("Mailbox limit reached.");
        }

        using var _ = _partitionKeyScope.Push(accountMap.Id.ToString());

        var routingKey = await _uniqueIdentifierGenerator.GenerateAsync(
            new UniqueIdentifierGeneratorSettings()
            {
                Length = 6,
                Chars = "ABCDEFHJKMNPQRSTUVWXYZ23456789"
            },
            async (key) =>
            {
                var existing = await _routingKeyMapRepository.FindByIdAsync(key, cancellationToken);
                return existing == null;
            });

        var mailbox = new Mailbox(
            accountId: accountMap.Id,
            routingKey: routingKey.ToLowerInvariant(),
            domain: "nullbox.email",
            name: request.Name,
            autoCreateAlias: request.AutoCreateAlias,
            emailAddress: request.EmailAddress.Trim().ToLowerInvariant());

        var defaultUserProfile = account.Users.First();

        mailbox.AddUser(
            userProfileId: defaultUserProfile.UserProfileId,
            roleId: defaultUserProfile.RoleId);

        _mailboxRepository.Add(mailbox);

        return $"{mailbox.RoutingKey}.{mailbox.Domain}";
    }
}
