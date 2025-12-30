using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace Nullbox.Security.Domain.Contracts.Users;

public record TurnstileResponse
{
    public TurnstileResponse(bool success,
        DateTimeOffset? challengeTimestamp,
        string? hostname,
        IEnumerable<string> errorCodes,
        string? action,
        string? cData,
        Dictionary<string, string>? metadata)
    {
        Success = success;
        ChallengeTimestamp = challengeTimestamp;
        Hostname = hostname;
        ErrorCodes = errorCodes;
        Action = action;
        CData = cData;
        Metadata = metadata;
    }

    /// <summary>
    /// Required by Entity Framework.
    /// </summary>
    protected TurnstileResponse()
    {
    }

    public bool Success { get; init; }
    public DateTimeOffset? ChallengeTimestamp { get; init; }
    public string? Hostname { get; init; }
    public IEnumerable<string> ErrorCodes { get; init; }
    public string? Action { get; init; }
    public string? CData { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}