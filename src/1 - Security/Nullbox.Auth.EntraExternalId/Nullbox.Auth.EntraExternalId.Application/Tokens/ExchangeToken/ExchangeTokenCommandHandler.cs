using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Auth.EntraExternalId.Application.IntegrationServices;
using Nullbox.Auth.EntraExternalId.Application.IntegrationServices.Contracts.Security.Tokens.Services.Tokens;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Auth.EntraExternalId.Application.Tokens.ExchangeToken;

public class ExchangeTokenCommandHandler : IRequestHandler<ExchangeTokenCommand, TokenContractDto>
{
    private readonly ITokensService _tokensService;

    public ExchangeTokenCommandHandler(ITokensService tokensService)
    {
        _tokensService = tokensService;
    }

    public async Task<TokenContractDto> Handle(ExchangeTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _tokensService.ExchangeTokenAsync(cancellationToken);

        // [IntentIgnore]
        return result;
    }
}