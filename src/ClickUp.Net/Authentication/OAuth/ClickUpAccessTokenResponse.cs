using System.Text.Json.Serialization;

namespace ClickUp.Net.Authentication.OAuth;

/// <summary>
/// Access token returned by the ClickUp OAuth token endpoint.
/// </summary>
public sealed class ClickUpAccessTokenResponse
{
    /// <summary>
    /// Gets or sets the OAuth access token. Do not log this value.
    /// ClickUp access tokens currently do not expire.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}
