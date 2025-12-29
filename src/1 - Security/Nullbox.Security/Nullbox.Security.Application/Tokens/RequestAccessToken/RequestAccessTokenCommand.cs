using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Nullbox.Security.Application.Tokens.RequestAccessToken;

public class RequestAccessTokenCommand : IRequest<TokenContractDto>, ICommand
{
    public RequestAccessTokenCommand(List<string> scopes)
    {
        Scopes = scopes;
    }

    public List<string> Scopes { get; set; }
}