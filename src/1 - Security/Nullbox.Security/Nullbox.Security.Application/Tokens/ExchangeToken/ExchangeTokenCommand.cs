using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Security.Application.Tokens.ExchangeToken;

public class ExchangeTokenCommand : IRequest<TokenContractDto>, ICommand
{
    public ExchangeTokenCommand()
    {
    }
}