using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Net.Infrastructure;
using ClickUp.Net.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ClickUp.Net.Authentication.OAuth;

internal sealed class ClickUpOAuthClient : IClickUpOAuthClient
{
    private const string AuthorizationEndpoint = "https://app.clickup.com/api";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<ClickUpOptions> _options;
    private readonly ILogger _logger;

    public ClickUpOAuthClient(IHttpClientFactory httpClientFactory, IOptions<ClickUpOptions> options, ILoggerFactory loggerFactory)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = loggerFactory.CreateLogger("ClickUp.Net.OAuth");
    }

    public string GetAuthorizationUrl(string clientId, string redirectUri)
    {
        return GetAuthorizationUrl(clientId, redirectUri, state: null);
    }

    public string GetAuthorizationUrl(string clientId, string redirectUri, string? state)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(clientId));
        }

        if (string.IsNullOrWhiteSpace(redirectUri))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(redirectUri));
        }

        var url = AuthorizationEndpoint + "?client_id=" + Uri.EscapeDataString(clientId) + "&redirect_uri=" + Uri.EscapeDataString(redirectUri);
        if (!string.IsNullOrEmpty(state))
        {
            url += "&state=" + Uri.EscapeDataString(state);
        }

        return url;
    }

    public async Task<ClickUpAccessTokenResponse> ExchangeCodeForTokenAsync(
        string clientId,
        string clientSecret,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(clientId));
        }

        if (string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(clientSecret));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(code));
        }

        var body = JsonSerializer.Serialize(new TokenRequest
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            Code = code
        }, ClickUpSerializer.Options);

        var http = new ClickUpHttpClient(_httpClientFactory, authenticator: null, _options, _logger);
        var response = await http.SendWithoutAuthenticationAsync<ClickUpAccessTokenResponse>(
            HttpMethod.Post,
            "oauth/token",
            body,
            cancellationToken).ConfigureAwait(false);

        if (response is null || string.IsNullOrWhiteSpace(response.AccessToken))
        {
            throw new InvalidOperationException("ClickUp did not return an access token.");
        }

        return response;
    }

    private sealed class TokenRequest
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
    }
}
