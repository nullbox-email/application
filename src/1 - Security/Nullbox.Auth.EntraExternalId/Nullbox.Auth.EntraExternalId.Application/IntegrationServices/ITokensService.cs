using Intent.RoslynWeaver.Attributes;
using Nullbox.Auth.EntraExternalId.Application.IntegrationServices.Contracts.Security.Tokens.Services.Tokens;

[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace Nullbox.Auth.EntraExternalId.Application.IntegrationServices;

public interface ITokensService : IDisposable
{
    Task<TokenContractDto> ExchangeTokenAsync(CancellationToken cancellationToken = default);
}