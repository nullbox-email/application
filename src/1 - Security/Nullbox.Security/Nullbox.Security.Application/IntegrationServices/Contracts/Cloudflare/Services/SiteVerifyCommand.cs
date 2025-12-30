using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace Nullbox.Security.Application.IntegrationServices.Contracts.Cloudflare.Services;

public class SiteVerifyCommand
{
    public SiteVerifyCommand()
    {
        Secret = null!;
        Response = null!;
    }

    public string Secret { get; set; }
    public string Response { get; set; }
    public string? RemoteIp { get; set; }
    public Guid? IdempotencyKey { get; set; }

    public static SiteVerifyCommand Create(string secret, string response, string? remoteIp, Guid? idempotencyKey)
    {
        return new SiteVerifyCommand
        {
            Secret = secret,
            Response = response,
            RemoteIp = remoteIp,
            IdempotencyKey = idempotencyKey
        };
    }
}