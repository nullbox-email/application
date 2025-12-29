using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Auth.EntraExternalId.Application.Common.Exceptions;
using Nullbox.Auth.EntraExternalId.Application.IntegrationServices;
using Nullbox.Auth.EntraExternalId.Application.IntegrationServices.Contracts.Security.Tokens.Services.Tokens;

[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace Nullbox.Auth.EntraExternalId.Infrastructure.HttpClients.Tokens;

public class TokensServiceHttpClient : ITokensService
{
    private const string JSON_MEDIA_TYPE = "application/json";
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public TokensServiceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                { new JsonStringEnumConverter() }
        };
    }

    public async Task<TokenContractDto> ExchangeTokenAsync(CancellationToken cancellationToken = default)
    {
        var relativeUri = $"v1/tokens/exchange";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativeUri);
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

        using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
        {
            if (!response.IsSuccessStatusCode)
            {
                throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
            }

            using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
            {
                return (await JsonSerializer.DeserializeAsync<TokenContractDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Class cleanup goes here
    }
}