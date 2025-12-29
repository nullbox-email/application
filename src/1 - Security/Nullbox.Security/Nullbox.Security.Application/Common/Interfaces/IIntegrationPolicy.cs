using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Endura.IntegrationPolicyInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Common.Interfaces;

public interface IIntegrationPolicy
{
    Guid SourceId { get; }
    Guid TargetId { get; }
    Guid CorrelationId { get; }
    void SetPolicy(Guid SourceId, Guid TargetId, Guid CorrelationId);
}