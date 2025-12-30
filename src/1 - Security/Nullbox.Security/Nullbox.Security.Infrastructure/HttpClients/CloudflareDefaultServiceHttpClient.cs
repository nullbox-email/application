using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Application.Common.Exceptions;
using Nullbox.Security.Application.IntegrationServices;
using Nullbox.Security.Application.IntegrationServices.Contracts.Cloudflare.Services;

[assembly: IntentTemplate("Intent.Integration.HttpClients.HttpClient", Version = "2.0")]

namespace Nullbox.Security.Infrastructure.HttpClients;

public class CloudflareDefaultServiceHttpClient : ICloudflareDefaultService
{
    private const string JSON_MEDIA_TYPE = "application/json";
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public CloudflareDefaultServiceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<TurnstileResponseDto> SiteVerifyAsync(
        SiteVerifyCommand command,
        CancellationToken cancellationToken = default)
    {
        var relativeUri = $"turnstile/v0/siteverify";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, relativeUri);
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

        var content = JsonSerializer.Serialize(command, _serializerOptions);
        httpRequest.Content = new StringContent(content, Encoding.UTF8, JSON_MEDIA_TYPE);

        using (var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
        {
            if (!response.IsSuccessStatusCode)
            {
                throw await HttpClientRequestException.Create(_httpClient.BaseAddress!, httpRequest, response, cancellationToken).ConfigureAwait(false);
            }

            using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
            {
                return (await JsonSerializer.DeserializeAsync<TurnstileResponseDto>(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false))!;
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