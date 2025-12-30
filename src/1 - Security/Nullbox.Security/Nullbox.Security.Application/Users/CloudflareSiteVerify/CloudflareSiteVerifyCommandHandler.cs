using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.IntegrationServices;
using Nullbox.Security.Application.IntegrationServices.Contracts.Cloudflare.Services;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Security.Application.Users.CloudflareSiteVerify;

public class CloudflareSiteVerifyCommandHandler : IRequestHandler<CloudflareSiteVerifyCommand>
{
    private readonly ICloudflareDefaultService _cloudflareDefaultService;

    public CloudflareSiteVerifyCommandHandler(ICloudflareDefaultService cloudflareDefaultService)
    {
        _cloudflareDefaultService = cloudflareDefaultService;
    }

    public async Task Handle(CloudflareSiteVerifyCommand request, CancellationToken cancellationToken)
    {
        var result = await _cloudflareDefaultService.SiteVerifyAsync(new SiteVerifyCommand
        {
            Secret = request.Secret,
            Response = request.Response,
            RemoteIp = request.RemoteIp,
            IdempotencyKey = request.IdempotencyKey
        }, cancellationToken);
    }
}