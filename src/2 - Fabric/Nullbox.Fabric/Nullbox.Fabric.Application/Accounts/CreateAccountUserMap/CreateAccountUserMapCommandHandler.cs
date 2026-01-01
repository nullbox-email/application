using System.Security.Principal;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Entities.Accounts;
using Nullbox.Fabric.Domain.Repositories.Accounts;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Accounts.CreateAccountUserMap;

public class CreateAccountUserMapCommandHandler : IRequestHandler<CreateAccountUserMapCommand>
{
    private readonly IAccountUserMapRepository _accountUserMapRepository;
    private readonly IPartitionKeyScope _partitionKeyScope;

    public CreateAccountUserMapCommandHandler(
        IAccountUserMapRepository accountUserMapRepository,
        IPartitionKeyScope partitionKeyScope)
    {
        _accountUserMapRepository = accountUserMapRepository;
        _partitionKeyScope = partitionKeyScope;
    }

    public async Task Handle(CreateAccountUserMapCommand request, CancellationToken cancellationToken)
    {
        // [IntentIgnore]
        using var _ = _partitionKeyScope.Push(request.PartitionKey.ToString());

        var accountUserMap = new AccountUserMap(
            id: request.Id,
            userId: request.PartitionKey);

        _accountUserMapRepository.Add(accountUserMap);
    }
}