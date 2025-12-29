using System.Net.Mime;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nullbox.Security.Application.OIDC;
using Nullbox.Security.Application.OIDC.GetJWKS;
using Nullbox.Security.Application.OIDC.GetOpenIdConfiguration;

[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Nullbox.Security.Api.Controllers;

[ApiController]
[Authorize]
public class OIDCController : ControllerBase
{
    private readonly ISender _mediator;

    public OIDCController(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// </summary>
    /// <response code="200">Returns the specified JwksDto.</response>
    [HttpGet(".well-known/jwks.json")]
    [AllowAnonymous]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(JwksDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<JwksDto>> GetJWKS(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetJWKSQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// </summary>
    /// <response code="200">Returns the specified OpenIdConfigurationDto.</response>
    [HttpGet(".well-known/openid-configuration")]
    [AllowAnonymous]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(OpenIdConfigurationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OpenIdConfigurationDto>> GetOpenIdConfiguration(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetOpenIdConfigurationQuery(), cancellationToken);
        return Ok(result);
    }
}