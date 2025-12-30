using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Contracts.Users;

[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace Nullbox.Security.Application.Services.Users;

public interface ITurnstileDomainService
{
    Task<TurnstileResponse> ValidateTokenAsync(string token, string remoteip, CancellationToken cancellationToken = default);
}