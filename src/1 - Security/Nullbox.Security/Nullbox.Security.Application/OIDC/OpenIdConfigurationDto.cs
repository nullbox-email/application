using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Security.Application.OIDC;

public class OpenIdConfigurationDto
{
    public OpenIdConfigurationDto()
    {
        Issuer = null!;
        JwksUri = null!;
        AuthorizationEndpoint = null!;
        TokenEndpoint = null!;
        UserInfoEndpoint = null!;
        ResponseTypesSupported = null!;
        IdTokenSigningAlgValuesSupported = null!;
    }

    public string Issuer { get; set; }
    public string JwksUri { get; set; }
    public string AuthorizationEndpoint { get; set; }
    public string TokenEndpoint { get; set; }
    public string UserInfoEndpoint { get; set; }
    public List<string> ResponseTypesSupported { get; set; }
    public List<string> IdTokenSigningAlgValuesSupported { get; set; }

    public static OpenIdConfigurationDto Create(
        string issuer,
        string jwksUri,
        string authorizationEndpoint,
        string tokenEndpoint,
        string userInfoEndpoint,
        List<string> responseTypesSupported,
        List<string> idTokenSigningAlgValuesSupported)
    {
        return new OpenIdConfigurationDto
        {
            Issuer = issuer,
            JwksUri = jwksUri,
            AuthorizationEndpoint = authorizationEndpoint,
            TokenEndpoint = tokenEndpoint,
            UserInfoEndpoint = userInfoEndpoint,
            ResponseTypesSupported = responseTypesSupported,
            IdTokenSigningAlgValuesSupported = idTokenSigningAlgValuesSupported
        };
    }
}