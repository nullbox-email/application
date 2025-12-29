using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Aliases;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Fabric.Domain.Entities.Aliases;

public partial class AliasRule
{
    public AliasRule(string id,
Guid aliasId,
AliasRuleKind ruleKind,
string domain,
string host,
string email,
DeliveryDecision decision,
bool isEnabled,
AliasRuleSource source)
    {
        Id = id;
        AliasId = aliasId;
        RuleKind = ruleKind;
        Domain = domain;
        Host = host;
        Email = email;
        Decision = decision;
        IsEnabled = isEnabled;
        Source = source;
    }
}