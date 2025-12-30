using System.Text.Json;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Nullbox.Security.Domain.Contracts.Users;

[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace Nullbox.Security.Domain.Services.Users;

public class TurnstileDomainService : ITurnstileDomainService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private const string SiteverifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

    public TurnstileDomainService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<TurnstileResponse> ValidateTokenAsync(
            string token,
            string remoteip,
            CancellationToken cancellationToken = default)
    {
        var secretKey = _configuration["Turnstyle:SecretKey"];

        var parameters = new Dictionary<string, string>
        {
            { "secret", secretKey },
            { "response", token }
        };

        if (!string.IsNullOrEmpty(remoteip))
        {
            parameters.Add("remoteip", remoteip);
        }

        var postContent = new FormUrlEncodedContent(parameters);

        try
        {
            var response = await _httpClient.PostAsync(SiteverifyUrl, postContent);
            var stringContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TurnstileResponse>(stringContent);
        }
        catch (Exception ex)
        {
            return new TurnstileResponse(
                success: false,
                challengeTimestamp: null,
                hostname: null,
                errorCodes: ["internal-error"],
                action: null, cData: null,
                metadata: []
            );
        }
    }
}