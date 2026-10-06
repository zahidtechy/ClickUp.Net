using ClickUp.Net.Authentication.OAuth;
using ClickUp.Net.Authentication.PersonalToken;
using ClickUp.Net.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ClickUp.Net;

internal sealed class ClickUpClientFactory : IClickUpClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<ClickUpOptions> _options;
    private readonly ILogger _logger;

    public ClickUpClientFactory(
        IHttpClientFactory httpClientFactory,
        IOptions<ClickUpOptions> options,
        ILoggerFactory loggerFactory)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
        _logger = loggerFactory.CreateLogger("ClickUp.Net.Http");
    }

    public IClickUpClient CreateDefault()
    {
        var options = _options.Value;
        var authenticator = options.AuthenticationType == Authentication.ClickUpAuthenticationType.OAuthAccessToken
            ? (Authentication.IClickUpAuthenticator)new OAuthAccessTokenAuthenticator(options.AccessToken)
            : new PersonalTokenAuthenticator(options.PersonalToken);
        return Create(authenticator);
    }

    public IClickUpClient CreateWithAccessToken(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(accessToken));
        }

        return Create(new OAuthAccessTokenAuthenticator(accessToken));
    }

    public IClickUpClient CreateWithPersonalToken(string personalToken)
    {
        if (string.IsNullOrWhiteSpace(personalToken))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(personalToken));
        }

        return Create(new PersonalTokenAuthenticator(personalToken));
    }

    private IClickUpClient Create(Authentication.IClickUpAuthenticator authenticator)
    {
        var http = new ClickUpHttpClient(_httpClientFactory, authenticator, _options, _logger);
        return new ClickUpClient(http);
    }
}
