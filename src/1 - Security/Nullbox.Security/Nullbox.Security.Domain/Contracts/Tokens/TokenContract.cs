using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace Nullbox.Security.Domain.Contracts.Tokens;

public record TokenContract
{
    public TokenContract(string? idToken, string? accessToken)
    {
        IdToken = idToken;
        AccessToken = accessToken;
    }

    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected TokenContract()
    {
    }

    public string? IdToken { get; init; }
    public string? AccessToken { get; init; }
}