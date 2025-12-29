using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class AliasSender
{
    public AliasSender(string id, Guid aliasId, string email, string domain, DeliveryDecision? lastDecision)
    {
        Id = id;
        AliasId = aliasId;
        Email = email;
        Domain = domain;
        LastDecision = lastDecision;
    }

    public void RecordSeen(DeliveryDecision? lastDecision, DateTimeOffset? seenAt)
    {
        SeenCount++;
        LastSeenAt = seenAt ?? DateTimeOffset.UtcNow;

        if (lastDecision is not null)
            LastDecision = lastDecision;
        LastDecision = lastDecision;
    }
}