using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Security.Domain.Entities.Users;

public partial class ExternalUser : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected ExternalUser()
    {
        Id = null!;
        Context = null!;
    }

    public string Id { get; private set; }

    public string Context { get; private set; }

    public Guid UserId { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}