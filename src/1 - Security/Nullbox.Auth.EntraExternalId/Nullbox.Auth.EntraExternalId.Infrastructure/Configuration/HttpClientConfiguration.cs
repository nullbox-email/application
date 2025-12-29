using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nullbox.Auth.EntraExternalId.Application.IntegrationServices;
using Nullbox.Auth.EntraExternalId.Infrastructure.HttpClients;
using Nullbox.Auth.EntraExternalId.Infrastructure.HttpClients.Tokens;

[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace Nullbox.Auth.EntraExternalId.Infrastructure.Configuration;

public static class HttpClientConfiguration
{
    public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services
            .AddHttpClient<ITokensService, TokensServiceHttpClient>(http =>
            {
                ApplyAppSettings(http, configuration, "Nullbox.Security.Tokens.Services", "TokensService");
            })
            // [IntentIgnore]
            .AddHeaders(config =>
            {
                config.AddFromHeader("Authorization");
            });
    }

    private static void ApplyAppSettings(
        HttpClient client,
        IConfiguration configuration,
        string groupName,
        string serviceName)
    {
        client.BaseAddress = configuration.GetValue<Uri>($"HttpClients:{serviceName}:Uri") ?? configuration.GetValue<Uri>($"HttpClients:{groupName}:Uri");
        client.Timeout = configuration.GetValue<TimeSpan?>($"HttpClients:{serviceName}:Timeout") ?? configuration.GetValue<TimeSpan?>($"HttpClients:{groupName}:Timeout") ?? TimeSpan.FromSeconds(100);
    }
}