using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Audit;
using Nullbox.Fabric.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Entities.Audit;

public partial class AuditLogEntry : IHasDomainEvent
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected AuditLogEntry()
    {
        PartitionKey = null!;
        Payload = null!;
        PayloadHash = null!;
        Name = null!;
        TraceId = null!;
    }

    public Guid Id { get; private set; }

    public string PartitionKey { get; private set; }

    public string Payload { get; private set; }

    public string PayloadHash { get; private set; }

    public AuditKind Kind { get; private set; }

    public string Name { get; private set; }

    public int DurationMs { get; private set; }

    public AuditStatus Status { get; private set; }

    public string? Error { get; private set; }

    public string TraceId { get; private set; }

    public Guid? UserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public List<DomainEvent> DomainEvents { get; set; } = [];
}