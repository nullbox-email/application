using Asp.Versioning;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nullbox.Auth.EntraExternalId.Application.IntegrationServices.Contracts.Security.Tokens.Services.Tokens;
using Nullbox.Auth.EntraExternalId.Application.Tokens.ExchangeToken;

[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
public class TokensController : ControllerBase
{
    private readonly ISender _mediator;

    public TokensController(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// </summary>
    /// <response code="201">Successfully created.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    [HttpPost("/v{version:apiVersion}/tokens/exchange")]
    [ProducesResponseType(typeof(TokenContractDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<TokenContractDto>> ExchangeToken(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ExchangeTokenCommand(), cancellationToken);
        return Created(string.Empty, result);
    }
}