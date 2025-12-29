using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace Nullbox.Auth.EntraExternalId.Application.IntegrationServices.Contracts.Security.Tokens.Services.Tokens;

public class TokenContractDto
{
    public string? IdToken { get; set; }
    public string? AccessToken { get; set; }

    public static TokenContractDto Create(string? idToken, string? accessToken)
    {
        return new TokenContractDto
        {
            IdToken = idToken,
            AccessToken = accessToken
        };
    }
}