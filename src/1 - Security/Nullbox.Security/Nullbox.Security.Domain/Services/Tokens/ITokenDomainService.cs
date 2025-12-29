using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Contracts.Tokens;

[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace Nullbox.Security.Domain.Services.Tokens;

public interface ITokenDomainService
{
    string CreateToken(string subject, IEnumerable<Claim> claims, string audience, int expiresInMinutes = 30);
    TokenContract GetIdToken(Guid userId, IEnumerable<Claim> claims, string audience, int expiresInMinutes = 60);
    TokenContract GetAccessToken(Guid userId, IEnumerable<string> authorizedScopes, IEnumerable<string> permissions, IEnumerable<Claim> claims, string audience, int expiresInMinutes = 5);
}