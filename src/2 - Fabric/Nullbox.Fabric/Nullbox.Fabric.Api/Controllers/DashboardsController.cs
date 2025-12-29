using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nullbox.Fabric.Application.Dashboards;
using Nullbox.Fabric.Application.Dashboards.GetDashboard;

[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Nullbox.Fabric.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
public class DashboardsController : ControllerBase
{
    private readonly ISender _mediator;

    public DashboardsController(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// </summary>
    /// <response code="200">Returns the specified DashboardDto.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    /// <response code="404">No DashboardDto could be found with the provided parameters.</response>
    [HttpGet("/v{version:apiVersion}/dashboard")]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<DashboardDto>> GetDashboard(
        [FromQuery] Guid? aliasId,
        [FromQuery] Guid? mailboxId,
        [FromQuery][Required] int number,
        [FromQuery] DashboardType type = DashboardType.Daily,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetDashboardQuery(aliasId: aliasId, mailboxId: mailboxId, number: number, type: type), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }
}