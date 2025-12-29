using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Accounts.CreateEnablementGrant;

public class CreateEnablementGrantCommand : IRequest, ICommand
{
    public CreateEnablementGrantCommand(Guid accountId)
    {
        AccountId = accountId;
    }

    public Guid AccountId { get; set; }
}