using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using Asp.Versioning;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nullbox.Fabric.Api.Controllers.FileTransfer;
using Nullbox.Fabric.Application.Deliveries;
using Nullbox.Fabric.Application.Deliveries.ProcessEmail;
using Nullbox.Fabric.Application.Deliveries.ProcessEmailComplete;
using Nullbox.Fabric.Application.Deliveries.QuarantineEmail;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace Nullbox.Fabric.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
public class DeliveriesController : ControllerBase
{
    private readonly ISender _mediator;
    [IntentIgnore]
    private readonly IConfiguration _configuration;

    [IntentManaged(Mode.Merge)]
    public DeliveriesController(ISender mediator, IConfiguration configuration)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _configuration = configuration;
    }

    /// <summary>
    /// </summary>
    /// <response code="201">Successfully created.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    [HttpPost("/v{version:apiVersion}/email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(DeliveryDecisionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    [IntentIgnore]
    public async Task<ActionResult<DeliveryDecisionDto>> ProcessEmail(
        [FromHeader(Name = "x-date")] string? signatureDate,
        [FromHeader(Name = "x-signature")] string? signature,
        [FromBody] ProcessEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1) Validate date format and freshness (prevents simple replay)
        if (!DateOnly.TryParse(signatureDate, out var date))
            return Unauthorized();

        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);
        if (date < todayUtc.AddDays(-1) || date > todayUtc.AddDays(1))
            return Unauthorized();

        // 2) Read raw body bytes (to hash exactly what was signed)
        // If you can’t easily get raw bytes here, sign a stable canonical JSON instead.
        var bodyBytes = HttpContext.Items["RawBodyBytes"] as byte[];
        if (bodyBytes is null) return StatusCode(500);

        var bodyHash = Sha256Hex(bodyBytes);

        var canonical = string.Join("\n", new[]
        {
            HttpContext.Request.Method,
            HttpContext.Request.Path.Value ?? "",
            signatureDate,
            bodyHash
        });

        var secret = _configuration["EmailIngress:HmacSecret"]; // store securely
        if (string.IsNullOrWhiteSpace(secret))
            return StatusCode(500);

        var expected = HmacBase64(secret, canonical);

        if (!FixedTimeEquals(expected, signature))
            return Unauthorized();

        // 3) Process
        var result = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, result);
    }

    /// <summary>
    /// </summary>
    /// <response code="201">Successfully created.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="404">One or more entities could not be found with the provided parameters.</response>
    [HttpPost("/v{version:apiVersion}/email/{deliveryActionId}/complete")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    [IntentIgnore]
    public async Task<ActionResult> ProcessEmailComplete(
        [FromHeader(Name = "x-date")] string? signatureDate,
        [FromHeader(Name = "x-signature")] string? signature,
        [FromRoute] Guid deliveryActionId,
        [FromBody] ProcessEmailCompleteCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.DeliveryActionId != deliveryActionId)
            return BadRequest();

        // 1) Validate date format and freshness (prevents simple replay)
        if (!DateOnly.TryParse(signatureDate, out var date))
            return Unauthorized();

        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);
        if (date < todayUtc.AddDays(-1) || date > todayUtc.AddDays(1))
            return Unauthorized();

        // 2) Read raw body bytes (to hash exactly what was signed)
        // If you can’t easily get raw bytes here, sign a stable canonical JSON instead.
        var bodyBytes = HttpContext.Items["RawBodyBytes"] as byte[];
        if (bodyBytes is null) return StatusCode(500);

        var bodyHash = Sha256Hex(bodyBytes);

        var canonical = string.Join("\n", new[]
        {
            HttpContext.Request.Method,
            HttpContext.Request.Path.Value ?? "",
            signatureDate,
            bodyHash
        });

        var secret = _configuration["EmailIngress:HmacSecret"]; // store securely
        if (string.IsNullOrWhiteSpace(secret))
            return StatusCode(500);

        var expected = HmacBase64(secret, canonical);

        if (!FixedTimeEquals(expected, signature))
            return Unauthorized();

        // 3) Process
        await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, null);
    }

    /// <summary>
    /// </summary>
    /// <response code="201">Successfully created.</response>
    /// <response code="400">One or more validation errors have occurred.</response>
    /// <response code="401">Unauthorized request.</response>
    /// <response code="403">Forbidden request.</response>
    /// <response code="404">One or more entities could not be found with the provided parameters.</response>
    [BinaryContent]
    [HttpPost("/v{version:apiVersion}/email/{deliveryActionId}/quarantine")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    [IntentIgnore]
    public async Task<ActionResult> QuarantineEmail(
        [FromHeader(Name = "x-date")] string? signatureDate,
        [FromHeader(Name = "x-signature")] string? signature,
        [FromRoute] Guid deliveryActionId,
        [FromQuery][Required] string partitionKey,
        [FromQuery][Required] QuarantineReason reason,
        [FromQuery][Required] string message,
        [FromHeader(Name = "Content-Type")] string? contentType,
        [FromHeader(Name = "Content-Length")] long? contentLength,
        CancellationToken cancellationToken = default)
    {
        // 1) Validate date format and freshness (prevents simple replay)
        if (!DateOnly.TryParse(signatureDate, out var date))
            return Unauthorized();

        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);
        if (date < todayUtc.AddDays(-1) || date > todayUtc.AddDays(1))
            return Unauthorized();

        // 2) Read raw body bytes (to hash exactly what was signed)
        // If you can’t easily get raw bytes here, sign a stable canonical JSON instead.
        var bodyBytes = HttpContext.Items["RawBodyBytes"] as byte[];
        if (bodyBytes is null) return StatusCode(500);

        var bodyHash = Sha256Hex(bodyBytes);

        var canonical = string.Join("\n", new[]
        {
            HttpContext.Request.Method,
            HttpContext.Request.Path.Value ?? "",
            signatureDate,
            bodyHash
        });

        var secret = _configuration["EmailIngress:HmacSecret"]; // store securely
        if (string.IsNullOrWhiteSpace(secret))
            return StatusCode(500);

        var expected = HmacBase64(secret, canonical);

        if (!FixedTimeEquals(expected, signature))
            return Unauthorized();

        Stream stream;
        string? filename = null;
        if (Request.Headers.TryGetValue("Content-Disposition", out var headerValues))
        {
            string? header = headerValues;
            if (header != null)
            {
                var contentDisposition = ContentDispositionHeaderValue.Parse(header);
                filename = contentDisposition?.FileName;
            }
        }

        if (Request.ContentType != null && (Request.ContentType == "application/x-www-form-urlencoded" || Request.ContentType.StartsWith("multipart/form-data")) && Request.Form.Files.Any())
        {
            var file = Request.Form.Files[0];
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");
            stream = file.OpenReadStream();
            filename ??= file.Name;
        }
        else
        {
            stream = Request.Body;
        }
        var command = new QuarantineEmailCommand(content: stream, filename: filename, deliveryActionId: deliveryActionId, partitionKey: partitionKey, reason: reason, message: message, contentType: contentType, contentLength: contentLength);

        await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, null);
    }

    [IntentIgnore]
    static string Sha256Hex(ReadOnlySpan<byte> data)
    {
        Span<byte> hash = stackalloc byte[32];
        System.Security.Cryptography.SHA256.HashData(data, hash);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    [IntentIgnore]
    static string HmacBase64(string secret, string msg)
    {
        var key = System.Text.Encoding.UTF8.GetBytes(secret);
        using var hmac = new System.Security.Cryptography.HMACSHA256(key);
        var sig = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(msg));
        return Convert.ToBase64String(sig);
    }

    [IntentIgnore]
    static bool FixedTimeEquals(string a, string b)
    {
        var ba = System.Text.Encoding.UTF8.GetBytes(a);
        var bb = System.Text.Encoding.UTF8.GetBytes(b);
        return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(ba, bb);
    }
}
