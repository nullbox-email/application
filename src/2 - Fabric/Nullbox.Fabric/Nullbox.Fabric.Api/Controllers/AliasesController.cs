using System.Net.Mime;
using Asp.Versioning;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nullbox.Fabric.Api.Controllers.ResponseTypes;
using Nullbox.Fabric.Application.Aliases.CreateAlias;
using Nullbox.Fabric.Application.Aliases.GetAliasById;
using Nullbox.Fabric.Application.Aliases.GetAliases;
using Nullbox.Fabric.Application.Aliases.UpdateAlias;
using Nullbox.Fabric.Application.Mailboxes;

[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Nullbox.Fabric.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
public class AliasesController : ControllerBase
{
    private readonly ISender _mediator;

    public AliasesController(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// </summary>
    /// <response code="201">Successfully created.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    /// <response code="404">One or more entities could not be found with the provided parameters.</response>
    [HttpPost("/v{version:apiVersion}/mailboxes/{mailboxId}/aliases")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(JsonResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<JsonResponse<string>>> CreateAlias(
        [FromRoute] Guid mailboxId,
        [FromBody] CreateAliasCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.MailboxId == Guid.Empty)
        {
            command.MailboxId = mailboxId;
        }

        if (mailboxId != command.MailboxId)
        {
            return BadRequest();
        }

        var result = await _mediator.Send(command, cancellationToken);
        return result == null ? NotFound() : Created(string.Empty, new JsonResponse<string>(result));
    }

    /// <summary>
    /// </summary>
    /// <response code="204">Successfully updated.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    /// <response code="404">One or more entities could not be found with the provided parameters.</response>
    [HttpPatch("/v{version:apiVersion}/mailboxes/{mailboxId}/aliases/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult> UpdateAlias(
        [FromRoute] Guid id,
        [FromRoute] Guid mailboxId,
        [FromBody] UpdateAliasCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.Id == Guid.Empty)
        {
            command.Id = id;
        }

        if (command.MailboxId == Guid.Empty)
        {
            command.MailboxId = mailboxId;
        }

        if (id != command.Id)
        {
            return BadRequest();
        }

        if (mailboxId != command.MailboxId)
        {
            return BadRequest();
        }

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// </summary>
    /// <response code="200">Returns the specified AliasDto.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    /// <response code="404">No AliasDto could be found with the provided parameters.</response>
    [HttpGet("/v{version:apiVersion}/mailboxes/{mailboxId}/aliases/{id}")]
    [ProducesResponseType(typeof(AliasDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<AliasDto>> GetAliasById(
        [FromRoute] string id,
        [FromRoute] string mailboxId,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAliasByIdQuery(id: id, mailboxId: mailboxId), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// </summary>
    /// <response code="200">Returns the specified List&lt;AliasDto&gt;.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    /// <response code="404">No List&lt;AliasDto&gt; could be found with the provided parameters.</response>
    [HttpGet("/v{version:apiVersion}/mailboxes/{mailboxId}/aliases")]
    [ProducesResponseType(typeof(List<AliasDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<List<AliasDto>>> GetAliases(
        [FromRoute] string mailboxId,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAliasesQuery(mailboxId: mailboxId), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }
}