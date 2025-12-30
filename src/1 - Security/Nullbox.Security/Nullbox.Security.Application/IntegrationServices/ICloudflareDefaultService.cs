using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Application.IntegrationServices.Contracts.Cloudflare.Services;

[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace Nullbox.Security.Application.IntegrationServices;

public interface ICloudflareDefaultService : IDisposable
{
    Task<TurnstileResponseDto> SiteVerifyAsync(SiteVerifyCommand command, CancellationToken cancellationToken = default);
}