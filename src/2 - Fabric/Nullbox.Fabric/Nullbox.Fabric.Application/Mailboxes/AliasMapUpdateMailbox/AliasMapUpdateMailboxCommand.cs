using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Fabric.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes.AliasMapUpdateMailbox;

public class AliasMapUpdateMailboxCommand : IRequest, ICommand
{
    public AliasMapUpdateMailboxCommand(Guid id,
        Guid accountId,
        string routingKey,
        string name,
        string domain,
        bool autoCreateAlias,
        string emailAddress)
    {
        Id = id;
        AccountId = accountId;
        RoutingKey = routingKey;
        Name = name;
        Domain = domain;
        AutoCreateAlias = autoCreateAlias;
        EmailAddress = emailAddress;
    }

    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string RoutingKey { get; set; }
    public string Name { get; set; }
    public string Domain { get; set; }
    public bool AutoCreateAlias { get; set; }
    public string EmailAddress { get; set; }
}