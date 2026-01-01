using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Audit;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Audit;

public partial class AuditLogEntry
{
    public AuditLogEntry(Guid id,
string partitionKey,
string payload,
string payloadHash,
AuditKind kind,
string name,
int durationMs,
AuditStatus status,
string? error,
string traceId,
Guid? userId)
    {
        Id = id;
        PartitionKey = partitionKey;
        Payload = payload;
        PayloadHash = payloadHash;
        Kind = kind;
        Name = name;
        DurationMs = durationMs;
        Status = status;
        Error = error;
        TraceId = traceId;
        UserId = userId;
    }
}