using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Nullbox.Security.Application.OIDC.GetOpenIdConfiguration;

public class GetOpenIdConfigurationQueryHandler : IRequestHandler<GetOpenIdConfigurationQuery, OpenIdConfigurationDto>
{
    private readonly IConfiguration _configuration;

    public GetOpenIdConfigurationQueryHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [IntentIgnore]
    public async Task<OpenIdConfigurationDto> Handle(
        GetOpenIdConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        var issuer = _configuration.GetSection("Security-Bearer:Issuer").Get<string>();

        if (string.IsNullOrEmpty(issuer))
        {
            throw new InvalidOperationException("Token issuer is not configured.");
        }

        var openIdConfiguration = new OpenIdConfigurationDto
        {
            Issuer = issuer,
            JwksUri = $"{issuer}/.well-known/jwks.json",
            AuthorizationEndpoint = $"{issuer}/connect/authorize",  // if relevant
            TokenEndpoint = $"{issuer}/connect/token",              // if relevant
            UserInfoEndpoint = $"{issuer}/connect/userinfo",        // if relevant
            ResponseTypesSupported = ["token", "id_token"],
            IdTokenSigningAlgValuesSupported = [SecurityAlgorithms.HmacSha256]
        };

        return openIdConfiguration;
    }
}