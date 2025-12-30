using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Security.Application.Users.CloudflareSiteVerify;

public class CloudflareSiteVerifyCommand : IRequest, ICommand
{
    public CloudflareSiteVerifyCommand(string secret, string response, string? remoteIp, Guid? idempotencyKey)
    {
        Secret = secret;
        Response = response;
        RemoteIp = remoteIp;
        IdempotencyKey = idempotencyKey;
    }

    public string Secret { get; set; }
    public string Response { get; set; }
    public string? RemoteIp { get; set; }
    public Guid? IdempotencyKey { get; set; }
}