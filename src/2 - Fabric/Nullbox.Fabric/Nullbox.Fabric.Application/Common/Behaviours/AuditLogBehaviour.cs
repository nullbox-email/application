// AuditLogBehaviour.cs
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nullbox.Fabric.Application.Common.Exceptions;
using Nullbox.Fabric.Application.Common.Interfaces;
using Nullbox.Fabric.Application.Common.Partitioning;
using Nullbox.Fabric.Domain.Audit;
using Nullbox.Fabric.Domain.Common.Interfaces;
using Nullbox.Fabric.Domain.Entities.Audit;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.AuditLogBehaviour", Version = "1.0")]

namespace Nullbox.Fabric.Application.Common.Behaviours;

/// <summary>
/// Writes an <see cref="AuditLogEntry"/> for commands and queries.
///
/// Notes on persistence:
/// - For commands (ICommand): this behaviour only stages the AuditLogEntry in the DbContext. It relies on UnitOfWorkBehaviour to SaveChanges,
///   so the audit entry commits atomically with the command's other writes (assuming same Cosmos container + same partition key).
/// - For queries (non-ICommand): there is no UnitOfWorkBehaviour, so this behaviour calls SaveChanges to persist the audit entry.
/// </summary>
public class AuditLogBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPartitionKeyScope _partitionKeyScope;
    private readonly ILogger<AuditLogBehaviour<TRequest, TResponse>> _logger;

    private readonly bool _enabled;
    private readonly bool _auditQueries;
    private readonly bool _includePayload;
    private readonly bool _persistFailedQueries;
    private readonly int _maxPayloadChars;

    public AuditLogBehaviour(
        IUnitOfWork uow,
        ICurrentUserService currentUserService,
        IPartitionKeyScope partitionKeyScope,
        ILogger<AuditLogBehaviour<TRequest, TResponse>> logger,
        IConfiguration configuration)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _partitionKeyScope = partitionKeyScope;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Defaults chosen to be safe; adjust as you prefer.
        _enabled = configuration.GetValue<bool?>("CqrsSettings:AuditLogEnabled") ?? true;
        _auditQueries = configuration.GetValue<bool?>("CqrsSettings:AuditLogQueries") ?? true;
        _includePayload = configuration.GetValue<bool?>("CqrsSettings:AuditLogRequestPayload") ?? true;

        // Only affects queries (commands that fail will not SaveChanges due to UnitOfWorkBehaviour).
        _persistFailedQueries = configuration.GetValue<bool?>("CqrsSettings:AuditLogFailedQueries") ?? true;

        // Cosmos item size limit exists; keep some headroom.
        _maxPayloadChars = configuration.GetValue<int?>("CqrsSettings:AuditLogMaxPayloadChars") ?? 50_000;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_enabled)
        {
            return await next(cancellationToken);
        }

        var isCommand = request is ICommand;
        if (!isCommand && !_auditQueries)
        {
            return await next(cancellationToken);
        }

        if (_uow is not DbContext db)
        {
            // If your IUnitOfWork is not a DbContext, you need a repository/DbContext dedicated to audit writes.
            return await next(cancellationToken);
        }

        var activity = Activity.Current;
        var traceId = activity?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
        var kind = isCommand ? AuditKind.Command : AuditKind.Query;
        var requestName = typeof(TRequest).FullName ?? $"Unknown{kind}";

        var currentUser = await _currentUserService.GetAsync();
        Guid? userId = null;

        if (Guid.TryParse(currentUser?.Id, out var _userId))
        {
            userId = _userId;
        }

        var partitionKey = _partitionKeyScope.Current ?? ResolvePartitionKey(request, userId);

        var started = Stopwatch.GetTimestamp();

        try
        {
            var response = await next(cancellationToken);

            var durationMs = ElapsedMs(started);
            var payload = BuildPayload(request);
            var payloadHash = Sha256Hex(payload);

            db.Set<AuditLogEntry>().Add(new AuditLogEntry(
                id: Guid.NewGuid(),
                partitionKey: partitionKey,
                payload: payload,
                payloadHash: payloadHash,
                kind: kind,
                name: requestName,
                durationMs: durationMs,
                status: AuditStatus.Succeeded,
                error: null,
                traceId: traceId,
                userId: userId));

            if (!isCommand)
            {
                // Queries have no UnitOfWorkBehaviour; persist immediately.
                await _uow.SaveChangesAsync(cancellationToken);
            }

            return response;
        }
        catch (Exception ex)
        {
            var durationMs = ElapsedMs(started);

            if (!isCommand && _persistFailedQueries)
            {
                try
                {
                    var payload = BuildPayload(request);
                    var payloadHash = Sha256Hex(payload);

                    db.Set<AuditLogEntry>().Add(new AuditLogEntry(
                        id: Guid.NewGuid(),
                        partitionKey: partitionKey,
                        payload: payload,
                        payloadHash: payloadHash,
                        kind: kind,
                        name: requestName,
                        durationMs: durationMs,
                        status: AuditStatus.Failed,
                        error: FormatError(ex),
                        traceId: traceId,
                        userId: userId));

                    await _uow.SaveChangesAsync(cancellationToken);
                }
                catch (Exception persistEx)
                {
                    _logger.LogWarning(persistEx, "AuditLog failed to persist failed query audit entry for {RequestName}", requestName);
                }
            }

            // For commands: do not attempt to persist a Failed audit entry here, because it breaks the
            // transactional-consistency goal (and UnitOfWorkBehaviour will not SaveChanges on failure).
            throw;
        }
    }

    private string BuildPayload(TRequest request)
    {
        if (!_includePayload)
        {
            return "{}";
        }

        string json;
        try
        {
            json = JsonSerializer.Serialize(request, JsonOptions);
        }
        catch (Exception ex)
        {
            return $"{{\"serializationError\":\"{EscapeForJson(ex.GetType().Name)}\"}}";
        }

        if (_maxPayloadChars > 0 && json.Length > _maxPayloadChars)
        {
            return json[.._maxPayloadChars];
        }

        return json;
    }

    private static int ElapsedMs(long startedTimestamp)
    {
        var elapsed = Stopwatch.GetElapsedTime(startedTimestamp);
        if (elapsed.TotalMilliseconds <= 0) return 0;
        if (elapsed.TotalMilliseconds >= int.MaxValue) return int.MaxValue;
        return (int)elapsed.TotalMilliseconds;
    }

    private static string Sha256Hex(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string FormatError(Exception ex)
    {
        // Keep it compact and avoid dumping sensitive data.
        var msg = ex.Message ?? string.Empty;
        if (msg.Length > 500) msg = msg[..500];
        return $"{ex.GetType().Name}: {msg}";
    }

    private static string EscapeForJson(string s) =>
        s.Replace("\\", "\\\\").Replace("\"", "\\\"");

    private static string ResolvePartitionKey(TRequest request, Guid? userId)
    {
        // Goal: match your existing transactional boundary (usually account/tenant partition key).
        // Heuristics (in order):
        // 1) request.PartitionKey (string)
        // 2) request.AccountId / MailboxId / AliasId (Guid or string)
        // 3) userId
        // 4) "global" fallback

        var pk =
            TryGetStringProperty(request, "PartitionKey") ??
            ToStringOrNull(TryGetProperty(request, "AccountId")) ??
            ToStringOrNull(TryGetProperty(request, "MailboxId")) ??
            ToStringOrNull(TryGetProperty(request, "AliasId")) ??
            (userId?.ToString());

        pk ??= "global";

        // Enforce max length 64 (your domain constraint).
        if (pk.Length > 64) pk = pk[..64];

        return pk;
    }

    private static object? TryGetProperty(object obj, string name)
    {
        var prop = obj.GetType().GetProperty(name);
        return prop?.GetValue(obj);
    }

    private static string? TryGetStringProperty(object obj, string name)
    {
        var val = TryGetProperty(obj, name);
        return val as string;
    }

    private static string? ToStringOrNull(object? val)
    {
        if (val is null) return null;
        if (val is string s) return string.IsNullOrWhiteSpace(s) ? null : s;
        return val.ToString();
    }
}
