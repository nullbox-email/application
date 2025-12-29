using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Auth.EntraExternalId.Application.Common.Interfaces;
using Nullbox.Auth.EntraExternalId.Application.IntegrationServices.Contracts.Security.Tokens.Services.Tokens;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Application.Tokens.ExchangeToken;

public class ExchangeTokenCommand : IRequest<TokenContractDto>, ICommand
{
    public ExchangeTokenCommand()
    {
    }
}