using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Duende.IdentityModel;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nullbox.Security.Domain.Contracts.Tokens;
using Nullbox.Security.Domain.Entities.Tokens;
using Nullbox.Security.Domain.Repositories.Tokens;

[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace Nullbox.Security.Domain.Services.Tokens;

public class TokenDomainService : ITokenDomainService
{
    private readonly ITokenRepository _tokenRepository;
    private readonly IConfiguration _configuration;

    public TokenDomainService(ITokenRepository tokenRepository, IConfiguration configuration)
    {
        _tokenRepository = tokenRepository;
        _configuration = configuration;
    }

    public string CreateToken(
        string subject,
        IEnumerable<System.Security.Claims.Claim> claims,
        string audience,
        int expiresInMinutes = 30)
    {
        var issuer = _configuration.GetSection("Security-Bearer:Issuer").Get<string>();
        var issuerSigningKey = _configuration.GetSection("Security-Bearer:IssuerSigningKey").Get<string>();
        var hmacKey = _configuration.GetSection("Security-Bearer:HmacKey").Get<string>();

        if (string.IsNullOrEmpty(issuer))
        {
            throw new InvalidOperationException("Token issuer is not configured.");
        }

        if (string.IsNullOrEmpty(issuerSigningKey))
        {
            throw new InvalidOperationException("Token issuer signing key is not configured.");
        }

        if (string.IsNullOrEmpty(hmacKey))
        {
            throw new InvalidOperationException("Token HMAC signing key is not configured.");
        }

        ArgumentNullException.ThrowIfNullOrWhiteSpace(subject);

        var jti = Guid.CreateVersion7();

        var tokenClaims = new List<System.Security.Claims.Claim>
        {
            new(JwtClaimTypes.JwtId, jti.ToString()),
            new(JwtClaimTypes.Subject, subject),
        };

        // Add additional claims to the token
        tokenClaims.AddRange(
            claims.Select(m =>
                new System.Security.Claims.Claim(m.Type, m.Value, m.ValueType, issuer)));

        var notBefore = DateTimeOffset.UtcNow;
        var expiresAt = notBefore.AddMinutes(expiresInMinutes);

        var symmetricSecurityKeyBytes = Encoding.UTF8.GetBytes(issuerSigningKey);
        var symmetricSecurityKey = new SymmetricSecurityKey(symmetricSecurityKeyBytes)
        {
            KeyId = hmacKey,
        };
        var credentials = new SigningCredentials(
            symmetricSecurityKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: tokenClaims,
            notBefore: notBefore.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var serializedToken = tokenHandler.WriteToken(token);

        var tokenEntity = new Token(
            id: jti,
            serializedToken: serializedToken,
            subject: subject,
            issuer: issuer,
            audience: audience,
            issuedAt: notBefore.UtcDateTime,
            expiresAt: expiresAt.UtcDateTime,
            notBefore: expiresAt.UtcDateTime
        );

        claims.ToList().ForEach(c => tokenEntity.AddClaim(c.Type, c.Value));

        _tokenRepository.Add(tokenEntity);

        return serializedToken;
    }

    public TokenContract GetIdToken(
        Guid userId,
        IEnumerable<System.Security.Claims.Claim> claims,
        string audience,
        int expiresInMinutes = 60)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User Id cannot be empty.", nameof(userId));
        }

        var token = CreateToken(
            subject: userId.ToString(),
            claims: claims,
            audience: audience,
            expiresInMinutes: expiresInMinutes
        );

        return new TokenContract(accessToken: null, idToken: token);
    }

    public TokenContract GetAccessToken(
        Guid userId,
        IEnumerable<string> authorizedScopes,
        IEnumerable<string> permissions,
        IEnumerable<System.Security.Claims.Claim> claims,
        string audience,
        int expiresInMinutes = 5)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User Id cannot be empty.", nameof(userId));
        }

        var tokenClaims = new List<System.Security.Claims.Claim>();

        if (authorizedScopes != null && authorizedScopes.Any())
        {
            var scopeClaimValue = string.Join(" ", authorizedScopes);
            tokenClaims.Add(new System.Security.Claims.Claim("scope", scopeClaimValue));
        }

        if (permissions != null && permissions.Any())
        {
            var permissionClaimValue = string.Join(" ", permissions);
            tokenClaims.Add(new System.Security.Claims.Claim("permission", permissionClaimValue));
        }

        if (claims != null)
        {
            tokenClaims.AddRange(claims);
        }

        var token = CreateToken(
            subject: userId.ToString(),
            claims: tokenClaims,
            audience: audience,
            expiresInMinutes: expiresInMinutes
        );

        return new TokenContract(accessToken: token, idToken: null);
    }
}