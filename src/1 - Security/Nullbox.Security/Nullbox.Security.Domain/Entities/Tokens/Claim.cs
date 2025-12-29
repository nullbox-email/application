using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Security.Domain.Entities.Tokens;

public partial class Claim
{
    public Claim(string type, string value)
    {
        Type = type;
        Value = value;
    }
}