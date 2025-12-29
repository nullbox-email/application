using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Security.Application.Tokens;

public class TokenContractDto
{
    public TokenContractDto()
    {
    }

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