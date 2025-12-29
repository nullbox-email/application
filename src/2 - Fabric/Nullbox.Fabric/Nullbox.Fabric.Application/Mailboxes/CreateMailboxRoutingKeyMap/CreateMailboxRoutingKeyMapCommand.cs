using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.CreateMailboxRoutingKeyMap;

public class CreateMailboxRoutingKeyMapCommand : IRequest, ICommand
{
    public CreateMailboxRoutingKeyMapCommand(string routingKey, Guid id, Guid userId)
    {
        RoutingKey = routingKey;
        Id = id;
        UserId = userId;
    }

    public string RoutingKey { get; set; }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}