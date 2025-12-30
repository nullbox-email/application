using System.Text.Json;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Nullbox.Security.Application.IntegrationServices;
using Nullbox.Security.Domain.Contracts.Users;

[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace Nullbox.Security.Application.Services.Users;

public class TurnstileDomainService : ITurnstileDomainService
{
    private readonly ICloudflareDefaultService _cloudflareDefaultService;
    private readonly IConfiguration _configuration;
    private const string SiteverifyUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

    public TurnstileDomainService(ICloudflareDefaultService cloudflareDefaultService, IConfiguration configuration)
    {
        _cloudflareDefaultService = cloudflareDefaultService;
        _configuration = configuration;
    }

    public async Task<TurnstileResponse> ValidateTokenAsync(
            string token,
            string remoteip,
            CancellationToken cancellationToken = default)
    {
        var secretKey = _configuration["Turnstile:SecretKey"];

        var postContent = new IntegrationServices.Contracts.Cloudflare.Services.SiteVerifyCommand()
        {
            Secret = secretKey,
            Response = token,
            RemoteIp = remoteip
        };

        try
        {
            var response = await _cloudflareDefaultService.SiteVerifyAsync(postContent, cancellationToken);

            return new TurnstileResponse(
                success: response.Success,
                challengeTimestamp: response.ChallengeTimestamp,
                hostname: response.Hostname,
                errorCodes: response.ErrorCodes,
                action: response.Action,
                cData: response.CData,
                metadata: response.Metadata
            );
        }
        catch (Exception ex)
        {
            return new TurnstileResponse(
                success: false,
                challengeTimestamp: null,
                hostname: null,
                errorCodes: ["internal-error"],
                action: null, 
                cData: null,
                metadata: []
            );
        }
    }
}