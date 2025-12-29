using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Security;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Repositories.Accounts;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Accounts.CreateAccount;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountRoleRepository _accountRoleRepository;

    public CreateAccountCommandHandler(
        IAccountRepository accountRepository,
        IAccountRoleRepository accountRoleRepository)
    {
        _accountRepository = accountRepository;
        _accountRoleRepository = accountRoleRepository;
    }

    [IntentIgnore]
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var scopes = new Scopes();

        var defaultRole = new AccountRole("Owner", [
            .. scopes.All.Select(x => x.Name.ToString())
        ]);

        var account = new Account(
            userProfileId: request.Id, name: request.Name, emailAddress: request.EmailAddress, adminRoleId: defaultRole.Id);

        _accountRepository.Add(account);
        _accountRoleRepository.Add(defaultRole);

        return account.Id;
    }
}