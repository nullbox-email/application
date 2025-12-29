using System.Net.Mime;
using Asp.Versioning;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nullbox.Fabric.Api.Controllers.ResponseTypes;
using Nullbox.Fabric.Application.Mailboxes;
using Nullbox.Fabric.Application.Mailboxes.CreateMailbox;
using Nullbox.Fabric.Application.Mailboxes.GetUserMailboxByRoutingKeyAndDomain;
using Nullbox.Fabric.Application.Mailboxes.GetUserMailboxes;
using Nullbox.Fabric.Application.Mailboxes.UpdateMailbox;

[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Nullbox.Fabric.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
public class MailboxesController : ControllerBase
{
    private readonly ISender _mediator;

    public MailboxesController(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// </summary>
    /// <response code="201">Successfully created.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    [HttpPost("/v{version:apiVersion}/mailboxes")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<JsonResponse<string>>> CreateMailbox(
        [FromBody] CreateMailboxCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetUserMailboxByRoutingKeyAndDomain), new { id = result }, new JsonResponse<string>(result));
    }

    /// <summary>
    /// </summary>
    /// <response code="204">Successfully updated.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    /// <response code="404">One or more entities could not be found with the provided parameters.</response>
    [HttpPatch("/v{version:apiVersion}/mailboxes/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult> UpdateMailbox(
        [FromRoute] Guid id,
        [FromBody] UpdateMailboxCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Id == Guid.Empty)
        {
            command.Id = id;
        }

        if (id != command.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// </summary>
    /// <response code="200">Returns the specified UserMailboxDto.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    /// <response code="404">No UserMailboxDto could be found with the provided parameters.</response>
    [HttpGet("/v{version:apiVersion}/mailboxes/{id}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserMailboxDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<UserMailboxDto>> GetUserMailboxByRoutingKeyAndDomain(
        [FromRoute] string id,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUserMailboxByRoutingKeyAndDomainQuery(id: id), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// </summary>
    /// <response code="200">Returns the specified List&lt;MailboxDto&gt;.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    [HttpGet("/v{version:apiVersion}/mailboxes")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(List<MailboxDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<List<MailboxDto>>> GetUserMailboxes(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUserMailboxesQuery(), cancellationToken);
        return Ok(result);
    }
}