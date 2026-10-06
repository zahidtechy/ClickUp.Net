namespace ClickUp.Net.Authentication.OAuth;

/// <summary>
/// Builds the ClickUp OAuth authorization URL and exchanges an authorization code for an access token.
/// </summary>
public interface IClickUpOAuthClient
{
    /// <summary>
    /// Builds the URL that sends a user to ClickUp to authorize Workspaces.
    /// </summary>
    /// <param name="clientId">The OAuth app client id.</param>
    /// <param name="redirectUri">The redirect URI registered for the OAuth app.</param>
    /// <returns>The authorization URL.</returns>
    string GetAuthorizationUrl(string clientId, string redirectUri);

    /// <summary>
    /// Builds the authorization URL and includes a <c>state</c> value.
    /// </summary>
    /// <param name="clientId">The OAuth app client id.</param>
    /// <param name="redirectUri">The redirect URI registered for the OAuth app.</param>
    /// <param name="state">An optional value ClickUp returns on the redirect.</param>
    /// <returns>The authorization URL.</returns>
    string GetAuthorizationUrl(string clientId, string redirectUri, string? state);

    /// <summary>
    /// Exchanges an authorization code for an access token.
    /// </summary>
    /// <param name="clientId">The OAuth app client id.</param>
    /// <param name="clientSecret">The OAuth app client secret. Do not log this value.</param>
    /// <param name="code">The authorization code from the redirect. Do not log this value.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The access token response.</returns>
    Task<ClickUpAccessTokenResponse> ExchangeCodeForTokenAsync(
        string clientId,
        string clientSecret,
        string code,
        CancellationToken cancellationToken = default);
}
