using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes;

public class UserMailboxDto
{
    public UserMailboxDto()
    {
        RoutingKey = null!;
        Name = null!;
        Domain = null!;
        EmailAddress = null!;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RoutingKey { get; set; }
    public string Name { get; set; }
    public string Domain { get; set; }
    public bool AutoCreateAlias { get; set; }
    public string EmailAddress { get; set; }

    public static UserMailboxDto Create(
        Guid id,
        Guid userId,
        string routingKey,
        string name,
        string domain,
        bool autoCreateAlias,
        string emailAddress)
    {
        return new UserMailboxDto
        {
            Id = id,
            UserId = userId,
            RoutingKey = routingKey,
            Name = name,
            Domain = domain,
            AutoCreateAlias = autoCreateAlias,
            EmailAddress = emailAddress
        };
    }
}