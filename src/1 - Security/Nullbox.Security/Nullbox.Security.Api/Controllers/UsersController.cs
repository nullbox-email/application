using Asp.Versioning;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nullbox.Security.Application.Users.OnboardUser;

[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Nullbox.Security.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    private readonly ISender _mediator;

    public UsersController(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// </summary>
    /// <response code="201">Successfully created.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    [HttpPost("/v{version:apiVersion}/on-board/user")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult> OnboardUser(
            [FromBody] OnboardUserCommand command,
            CancellationToken cancellationToken = default)
    {
        await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, null);
    }
}