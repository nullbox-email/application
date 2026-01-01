using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Application.Security;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Repositories.Accounts;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Accounts.CreateAccount;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountRoleRepository _accountRoleRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public CreateAccountCommandHandler(
        IAccountRepository accountRepository,
        IAccountRoleRepository accountRoleRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _accountRepository = accountRepository;
        _accountRoleRepository = accountRoleRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    [IntentIgnore]
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {

        var scopes = new Scopes();

        var defaultRoleId = Guid.NewGuid();

        var account = new Account(
            userProfileId: request.Id, 
            name: request.Name, 
            emailAddress: request.EmailAddress, 
            adminRoleId: defaultRoleId);

        using var _ = _partitionKeyScope.Push(account.AccountId.ToString());

        var defaultRole = new AccountRole(
            id: defaultRoleId,
            accountId: account.Id,
            name: "Owner",
            scopes: [.. scopes.All.Select(x => x.Name.ToString())]);

        _accountRepository.Add(account);
        _accountRoleRepository.Add(defaultRole);

        return account.Id;
    }
}