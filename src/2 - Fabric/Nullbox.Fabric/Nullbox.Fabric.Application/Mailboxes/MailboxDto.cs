using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes;

public class MailboxDto
{
    public MailboxDto()
    {
        Name = null!;
        RoutingKey = null!;
        Domain = null!;
        Aliases = null!;
        EmailAddress = null!;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string RoutingKey { get; set; }
    public string Domain { get; set; }
    public List<AliasDto> Aliases { get; set; }
    public bool AutoCreateAlias { get; set; }
    public string EmailAddress { get; set; }

    public static MailboxDto Create(
            Guid id,
            string name,
            string routingKey,
            string domain,
            List<AliasDto> aliases,
            bool autoCreateAlias,
            string emailAddress)
    {
        return new MailboxDto
        {
            Id = id,
            Name = name,
            RoutingKey = routingKey,
            Domain = domain,
            Aliases = aliases,
            AutoCreateAlias = autoCreateAlias,
            EmailAddress = emailAddress
        };
    }
}