using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Events.Tokens;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Nullbox.Security.Domain.Entities.Tokens;

public partial class Token
{
    public Token(Guid id,
        string serializedToken,
        string subject,
        string audience,
        string issuer,
        DateTimeOffset issuedAt,
        DateTimeOffset expiresAt,
        DateTimeOffset notBefore)
    {
        Id = id;
        SerializedToken = serializedToken;
        Subject = subject;
        Audience = audience;
        Issuer = issuer;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
        NotBefore = notBefore;

        DomainEvents.Add(new TokenCreatedDomainEvent(
            token: this));
    }

    public void Revoke()
    {
        if (IsRevoked)
        {
            throw new InvalidOperationException("Token is already revoked.");
        }

        IsRevoked = true;

        DomainEvents.Add(new TokenRevokedDomainEvent(
            token: this));
    }

    public void AddClaim(string type, string value)
    {
        _claims.Add(new Claim(type, value));
    }
}