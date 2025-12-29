using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Nullbox.Security.Application.OIDC.GetOpenIdConfiguration;

public class GetOpenIdConfigurationQuery : IRequest<OpenIdConfigurationDto>, IQuery
{
    public GetOpenIdConfigurationQuery()
    {
    }
}