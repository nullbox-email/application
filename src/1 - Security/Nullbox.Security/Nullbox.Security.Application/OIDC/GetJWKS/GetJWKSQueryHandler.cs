using System.Text;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Nullbox.Security.Application.OIDC.GetJWKS;

public class GetJWKSQueryHandler : IRequestHandler<GetJWKSQuery, JwksDto>
{
    private readonly IConfiguration _configuration;

    public GetJWKSQueryHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [IntentIgnore]
    public async Task<JwksDto> Handle(GetJWKSQuery request, CancellationToken cancellationToken)
    {
        var issuerSigningKey = _configuration.GetSection("Security-Bearer:IssuerSigningKey").Get<string>();
        var hmacKey = _configuration.GetSection("Security-Bearer:HmacKey").Get<string>();

        if (string.IsNullOrEmpty(issuerSigningKey))
        {
            throw new InvalidOperationException("Token issuer signing key is not configured.");
        }

        if (string.IsNullOrEmpty(hmacKey))
        {
            throw new InvalidOperationException("Token HMAC signing key is not configured.");
        }

        var secretBytes = Encoding.UTF8.GetBytes(issuerSigningKey);
        var base64UrlSecret = Base64UrlEncoder.Encode(secretBytes);

        var symmetricSecurityKeyBytes = Encoding.UTF8.GetBytes(issuerSigningKey);
        var symmetricSecurityKey = new SymmetricSecurityKey(symmetricSecurityKeyBytes)
        {
            KeyId = hmacKey,
        };

        var jwks = new JwksDto
        {
            Keys = new List<JsonWebKeyDto>
            {
                new JsonWebKeyDto
                {
                    Kty = "oct",
                    Alg = SecurityAlgorithms.HmacSha256,
                    Use = "sig",
                    Kid = symmetricSecurityKey.KeyId,
                    K = base64UrlSecret,
                    KeyOps = ["sign", "verify"]
                }
            }
        };

        return jwks;
    }
}