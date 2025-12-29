using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class AliasMap : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected AliasMap()
    {
        Id = null!;
        EmailAddress = null!;
    }

    public string Id { get; private set; }

    public Guid MailboxId { get; private set; }

    public Guid AccountId { get; private set; }

    public Guid AliasId { get; private set; }

    public bool IsEnabled { get; private set; } = true;

    public bool DirectPassthrough { get; private set; } = false;

    public bool LearningMode { get; private set; } = true;

    public bool AutoCreateAlias { get; private set; } = true;

    public string EmailAddress { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}