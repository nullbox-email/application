using HealthChecks.UI.Client;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.Cosmos;

[assembly: IntentTemplate("Intent.AspNetCore.HealthChecks.HealthChecksConfiguration", Version = "1.0")]

namespace Nullbox.Security.Api.Configuration;

public static class HealthChecksConfiguration
{
    public static IServiceCollection ConfigureHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();
        // [IntentIgnore]
        hcBuilder.Services.AddSingleton(_ => new CosmosClient(configuration["ConnectionStrings:cosmos-db"]!));
        hcBuilder.AddAzureCosmosDB(name: "CosmosDb", tags: new[] { "database" });
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