using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Accounts.CreateAccountUserMap;

public class CreateAccountUserMapCommand : IRequest, ICommand
{
    public CreateAccountUserMapCommand(Guid id, Guid partitionKey)
    {
        Id = id;
        PartitionKey = partitionKey;
    }

    public Guid Id { get; set; }
    public Guid PartitionKey { get; set; }
}