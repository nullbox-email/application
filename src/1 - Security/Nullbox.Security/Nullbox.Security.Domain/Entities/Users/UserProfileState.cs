using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Common;
using Nullbox.Security.Domain.Users;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Security.Domain.Entities.Users;

public partial class UserProfile : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected UserProfile()
    {
        EmailAddress = null!;
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public string Name { get; private set; } = String.Empty;

    public EmailAddressValueObject EmailAddress { get; private set; }

    public UserStatus Status { get; private set; } = UserStatus.New;

    public List<DomainEvent> DomainEvents { get; set; } = [];
}