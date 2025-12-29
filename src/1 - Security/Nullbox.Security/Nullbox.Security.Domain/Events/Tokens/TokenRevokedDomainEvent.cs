using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Common;
using Nullbox.Security.Domain.Entities.Tokens;

[assembly: IntentTemplate("Intent.DomainEvents.DomainEvent", Version = "1.0")]

namespace Nullbox.Security.Domain.Events.Tokens;

public class TokenRevokedDomainEvent : DomainEvent
{
    public TokenRevokedDomainEvent(Token token)
    {
        Token = token;
    }

    public Token Token { get; }
}