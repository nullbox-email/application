using HealthChecks.UI.Client;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

[assembly: IntentTemplate("Intent.AspNetCore.HealthChecks.HealthChecksConfiguration", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Api.Configuration;

public static class HealthChecksConfiguration
{
    public static IServiceCollection ConfigureHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();
        hcBuilder.AddApplicationInsightsPublisher(connectionString: configuration["ApplicationInsights:ConnectionString"]);

        return services;
    }

    public static IEndpointRouteBuilder MapDefaultHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        return endpoints;
    }
}