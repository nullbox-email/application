using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Security.Domain.Entities.Tokens;

public partial class Claim
{
    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected Claim()
    {
        Type = null!;
        Value = null!;
    }

    public Guid TokenId { get; private set; }

    public string Type { get; private set; }

    public string Value { get; private set; }
}