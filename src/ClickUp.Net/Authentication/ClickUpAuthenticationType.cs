namespace ClickUp.Net.Authentication;

/// <summary>
/// Selects how the ClickUp client authenticates requests.
/// </summary>
public enum ClickUpAuthenticationType
{
    /// <summary>
    /// Sends the personal API token in the <c>Authorization</c> header without a scheme.
    /// </summary>
    PersonalToken = 0,

    /// <summary>
    /// Sends an OAuth access token as <c>Authorization: Bearer {token}</c>.
    /// </summary>
    OAuthAccessToken = 1
}
