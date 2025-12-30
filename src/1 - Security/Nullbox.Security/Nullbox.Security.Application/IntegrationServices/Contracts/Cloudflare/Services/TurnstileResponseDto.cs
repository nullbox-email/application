using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace Nullbox.Security.Application.IntegrationServices.Contracts.Cloudflare.Services;

public class TurnstileResponseDto
{
    [IntentIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("success")]
    public bool Success { get; set; }

    [IntentIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("challenge_ts")]
    public DateTimeOffset? ChallengeTimestamp { get; set; }

    [IntentIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("hostname")]
    public string? Hostname { get; set; }

    [IntentIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("action")]
    public string? Action { get; set; }

    [IntentIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("cdata")]
    public string? CData { get; set; }

    [IntentIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("error-codes")]
    public List<string> ErrorCodes { get; set; } = [];

    [IntentIgnore]
    [System.Text.Json.Serialization.JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = [];

    public static TurnstileResponseDto Create(
        bool success,
        DateTimeOffset? challengeTimestamp,
        string? hostname,
        string? action,
        string? cData,
        List<string> errorCodes,
        Dictionary<string, object> metadata)
    {
        return new TurnstileResponseDto
        {
            Success = success,
            ChallengeTimestamp = challengeTimestamp,
            Hostname = hostname,
            Action = action,
            CData = cData,
            ErrorCodes = errorCodes,
            Metadata = metadata
        };
    }
}