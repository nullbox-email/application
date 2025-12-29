using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class AliasSenderDecision : IHasDomainEvent
{
    public string Id { get; private set; }

    public Guid AliasId { get; private set; }

    public DateTimeOffset LearningUntil { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}