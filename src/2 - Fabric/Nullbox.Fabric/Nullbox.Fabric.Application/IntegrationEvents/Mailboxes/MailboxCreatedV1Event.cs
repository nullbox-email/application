using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventMessage", Version = "1.0")]

namespace Nullbox.Fabric.Mailboxes.Eventing.Messages.Mailboxes;

public record MailboxCreatedV1Event
{
    public MailboxCreatedV1Event()
    {
        RoutingKey = null!;
        Name = null!;
        Domain = null!;
        EmailAddress = null!;
    }

    public Guid AccountId { get; init; }
    public Guid Id { get; init; }
    public string RoutingKey { get; init; }
    public string Name { get; init; }
    public string Domain { get; init; }
    public bool AutoCreateAlias { get; init; }
    public string EmailAddress { get; init; }
}