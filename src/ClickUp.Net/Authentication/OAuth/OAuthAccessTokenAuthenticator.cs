using ClickUp.Net.Authentication;

namespace ClickUp.Net.Authentication.OAuth;

internal sealed class OAuthAccessTokenAuthenticator : IClickUpAuthenticator
{
    private readonly string? _accessToken;

    public OAuthAccessTokenAuthenticator(string? accessToken)
    {
        _accessToken = accessToken;
    }

    public string CreateAuthorizationHeader()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            throw new InvalidOperationException("ClickUp OAuth access token is not configured.");
        }

        const string prefix = "Bearer ";
        if (_accessToken.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return _accessToken;
        }

        return prefix + _accessToken;
    }
}
