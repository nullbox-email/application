using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Repositories.Accounts;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Accounts.CreateAccountUserMap;

public class CreateAccountUserMapCommandHandler : IRequestHandler<CreateAccountUserMapCommand>
{
    private readonly IAccountUserMapRepository _accountUserMapRepository;

    public CreateAccountUserMapCommandHandler(IAccountUserMapRepository accountUserMapRepository)
    {
        _accountUserMapRepository = accountUserMapRepository;
    }

    public async Task Handle(CreateAccountUserMapCommand request, CancellationToken cancellationToken)
    {
        var accountUserMap = new AccountUserMap(
            id: request.Id,
            userId: request.PartitionKey);

        _accountUserMapRepository.Add(accountUserMap);
    }
}