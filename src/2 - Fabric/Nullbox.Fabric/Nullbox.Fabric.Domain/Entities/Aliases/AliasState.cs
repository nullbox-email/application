using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class Alias : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected Alias()
    {
        Name = null!;
        LocalPart = null!;
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public Guid MailboxId { get; private set; }

    public Guid AccountId { get; private set; }

    public string Name { get; private set; }

    public string LocalPart { get; private set; }

    public bool IsEnabled { get; private set; } = true;

    public bool DirectPassthrough { get; private set; } = false;

    public bool LearningMode { get; private set; } = true;

    public List<DomainEvent> DomainEvents { get; set; } = [];
}