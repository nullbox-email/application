using Azure.Monitor.OpenTelemetry.AspNetCore;
using Intent.RoslynWeaver.Attributes;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

[assembly: IntentTemplate("Intent.OpenTelemetry.OpenTelemetryConfiguration", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Api.Configuration;

public static class OpenTelemetryConfiguration
{
    public static IServiceCollection AddTelemetryConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry().UseAzureMonitor(opt =>
        {
            opt.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
        })
            .ConfigureResource(res => res
                .AddService(serviceName: configuration["OpenTelemetry:ServiceName"]!, serviceInstanceId: configuration.GetValue<string?>("OpenTelemetry:ServiceInstanceId"))
                .AddTelemetrySdk()
                .AddEnvironmentVariableDetector());
        return services;
    }
}