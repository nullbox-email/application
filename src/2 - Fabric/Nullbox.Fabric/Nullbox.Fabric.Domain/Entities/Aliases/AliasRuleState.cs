using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Aliases;
using Nullbox.Fabric.Domain.Common;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class AliasRule : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected AliasRule()
    {
        Id = null!;
        Domain = null!;
        Host = null!;
        Email = null!;
    }

    public string Id { get; private set; }

    public Guid AliasId { get; private set; }

    public AliasRuleKind RuleKind { get; private set; }

    public string Domain { get; private set; }

    public string Host { get; private set; }

    public string Email { get; private set; }

    public DeliveryDecision Decision { get; private set; }

    public bool IsEnabled { get; private set; } = true;

    public AliasRuleSource Source { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}