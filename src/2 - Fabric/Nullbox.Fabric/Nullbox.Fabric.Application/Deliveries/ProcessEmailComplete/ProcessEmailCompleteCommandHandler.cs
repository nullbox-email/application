using System.Diagnostics;
using System.Diagnostics.Metrics;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;
using Nullbox.Fabric.Domain.Deliveries;
using Nullbox.Fabric.Domain.Repositories.Deliveries;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Fabric.Application.Deliveries.ProcessEmailComplete;

public class ProcessEmailCompleteCommandHandler : IRequestHandler<ProcessEmailCompleteCommand>
{
    // Telemetry primitives (stable names)
    private static readonly ActivitySource ActivitySource = new("Nullbox.Fabric.ProcessEmailComplete");
    private static readonly Meter Meter = new("Nullbox.Fabric", "1.0.0");

    private static readonly Counter<long> TotalCompletions =
        Meter.CreateCounter<long>("email.complete.total", unit: "{completion}", description: "Total completion events processed");

    private static readonly Counter<long> CompletionNotFound =
        Meter.CreateCounter<long>("email.complete.not_found", unit: "{completion}", description: "Completion events where delivery action was not found");

    private static readonly Counter<long> CompletionByProviderStatus =
        Meter.CreateCounter<long>("email.complete.by_provider_status", unit: "{completion}", description: "Completions by provider status");

    private static readonly Histogram<double> CompletionDurationMs =
        Meter.CreateHistogram<double>("email.complete.duration", unit: "ms", description: "ProcessEmailComplete handler duration");

    private readonly IDeliveryActionRepository _deliveryActionRepository;
    private readonly ILogger<ProcessEmailCompleteCommandHandler> _logger;

    public ProcessEmailCompleteCommandHandler(
        IDeliveryActionRepository deliveryActionRepository,
        ILogger<ProcessEmailCompleteCommandHandler> logger)
    {
        _deliveryActionRepository = deliveryActionRepository;
        _logger = logger;
    }

    private static string Normalize(string? v) => (v ?? string.Empty).Trim();
    private static string NormalizeLower(string? v) => Normalize(v).ToLowerInvariant();

    public async Task Handle(ProcessEmailCompleteCommand request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        // Normalize inputs (store normalized values in the log/trace)
        var outcome = Normalize(request.Outcome);
        var outcomeNorm = NormalizeLower(request.Outcome);
        var provider = string.IsNullOrWhiteSpace(request.Provider) ? null : NormalizeLower(request.Provider);
        var providerMessageId = string.IsNullOrWhiteSpace(request.ProviderMessageId) ? null : Normalize(request.ProviderMessageId);
        var error = string.IsNullOrWhiteSpace(request.Error) ? null : Normalize(request.Error);

        using var activity = ActivitySource.StartActivity("ProcessEmailComplete", ActivityKind.Internal);

        // Trace tags (high cardinality is OK here; avoid in metrics)
        activity?.SetTag("email.delivery_action_id", request.DeliveryActionId);
        activity?.SetTag("email.partition_key", request.PartitionKey);
        activity?.SetTag("email.outcome", outcome);
        activity?.SetTag("email.provider", provider);

        // Metrics: total
        TotalCompletions.Add(1,
            new("outcome", string.IsNullOrWhiteSpace(outcomeNorm) ? "unknown" : outcomeNorm),
            new("provider", provider ?? "none"));

        try
        {
            var deliveryAction = await _deliveryActionRepository.FindAsync(
                a => a.Id == request.DeliveryActionId && a.PartitionKey == request.PartitionKey,
                cancellationToken);

            if (deliveryAction is null)
            {
                activity?.SetTag("email.complete.result", "not_found");

                CompletionNotFound.Add(1,
                    new("outcome", string.IsNullOrWhiteSpace(outcomeNorm) ? "unknown" : outcomeNorm),
                    new("provider", provider ?? "none"));

                _logger.LogWarning(
                    "ProcessEmailComplete: DeliveryAction not found. DeliveryActionId={DeliveryActionId} PartitionKey={PartitionKey} Outcome={Outcome} Provider={Provider}",
                    request.DeliveryActionId, request.PartitionKey, outcome, provider);

                return;
            }

            var providerStatus = outcomeNorm switch
            {
                "dropped" => ProviderStatus.NotAttempted,
                "quarantined" => ProviderStatus.NotAttempted,
                "forwarded" => ProviderStatus.Accepted,
                "forwardfailed" => ProviderStatus.Failed,
                _ => ProviderStatus.Failed,
            };

            // If no provider attempt happened, null out provider + message id.
            var providerForPersist = providerStatus == ProviderStatus.NotAttempted ? null : provider;
            var providerMessageIdForPersist = providerStatus == ProviderStatus.NotAttempted ? null : providerMessageId;

            deliveryAction.Complete(
                provider: providerForPersist,
                providerStatus: providerStatus,
                providerMessageId: providerMessageIdForPersist,
                providerError: error);

            _deliveryActionRepository.Update(deliveryAction);

            activity?.SetTag("email.complete.result", "updated");
            activity?.SetTag("email.provider_status", providerStatus.ToString());

            CompletionByProviderStatus.Add(1,
                new("provider_status", providerStatus.ToString().ToLowerInvariant()),
                new("outcome", string.IsNullOrWhiteSpace(outcomeNorm) ? "unknown" : outcomeNorm),
                new("provider", provider ?? "none"));

            _logger.LogInformation(
                "ProcessEmailComplete: Updated. ProviderStatus={ProviderStatus} Outcome={Outcome} Provider={Provider} DeliveryActionId={DeliveryActionId}",
                providerStatus, outcome, provider, deliveryAction.Id);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().FullName);
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("exception.stacktrace", ex.ToString());

            _logger.LogError(ex,
                "ProcessEmailComplete failed. DeliveryActionId={DeliveryActionId} PartitionKey={PartitionKey} Outcome={Outcome} Provider={Provider}",
                request.DeliveryActionId, request.PartitionKey, outcome, provider);

            throw;
        }
        finally
        {
            sw.Stop();

            CompletionDurationMs.Record(sw.Elapsed.TotalMilliseconds,
                new("outcome", string.IsNullOrWhiteSpace(outcomeNorm) ? "unknown" : outcomeNorm),
                new("provider", provider ?? "none"));

            activity?.SetTag("email.complete.duration_ms", sw.Elapsed.TotalMilliseconds);
        }
    }
}
