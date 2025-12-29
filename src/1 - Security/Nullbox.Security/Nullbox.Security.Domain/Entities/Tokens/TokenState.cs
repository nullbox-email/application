using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Common;

[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace Nullbox.Security.Domain.Entities.Tokens;

public partial class Token : IHasDomainEvent
{
    private List<Claim> _claims = [];

    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected Token()
    {
        SerializedToken = null!;
        Subject = null!;
        Audience = null!;
        Issuer = null!;
    }

    public Guid Id { get; private set; }

    public string SerializedToken { get; private set; }

    public string Subject { get; private set; }

    public string Audience { get; private set; }

    public string Issuer { get; private set; }

    public DateTimeOffset IssuedAt { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset NotBefore { get; private set; }

    public bool IsRevoked { get; private set; } = false;

    public virtual IReadOnlyCollection<Claim> Claims
    {
        get => _claims.AsReadOnly();
        private set => _claims = new List<Claim>(value);
    }

    public List<DomainEvent> DomainEvents { get; set; } = [];
}