namespace ClickUp.Net;

/// <summary>
/// Creates ClickUp clients that share the application's <see cref="IHttpClientFactory"/> pipeline.
/// </summary>
public interface IClickUpClientFactory
{
    /// <summary>
    /// Creates a client from the configured <see cref="ClickUpOptions"/>.
    /// </summary>
    /// <returns>A client authenticated with the configured personal token or OAuth access token.</returns>
    IClickUpClient CreateDefault();

    /// <summary>
    /// Creates a client that sends an OAuth access token as a bearer credential.
    /// </summary>
    /// <param name="accessToken">The OAuth access token. Do not log this value.</param>
    /// <returns>An authenticated client.</returns>
    IClickUpClient CreateWithAccessToken(string accessToken);

    /// <summary>
    /// Creates a client that sends a personal API token.
    /// </summary>
    /// <param name="personalToken">The personal API token. Do not log this value.</param>
    /// <returns>An authenticated client.</returns>
    IClickUpClient CreateWithPersonalToken(string personalToken);
}
