using ClickUp.Net.Authentication;

namespace ClickUp.Net;

/// <summary>
/// Configuration for the ClickUp API client.
/// </summary>
public sealed class ClickUpOptions
{
    /// <summary>
    /// The configuration section name used by <c>AddClickUp(IConfiguration)</c>.
    /// </summary>
    public const string SectionName = "ClickUp";

    /// <summary>
    /// The default ClickUp API v2 base address.
    /// </summary>
    public const string DefaultBaseUrl = "https://api.clickup.com/api/v2/";

    /// <summary>
    /// Gets or sets the API base address. A trailing slash is added when missing.
    /// </summary>
    public string BaseUrl { get; set; } = DefaultBaseUrl;

    /// <summary>
    /// Gets or sets the personal API token (<c>pk_...</c>).
    /// </summary>
    public string? PersonalToken { get; set; }

    /// <summary>
    /// Gets or sets an OAuth access token used when <see cref="AuthenticationType"/> is
    /// <see cref="ClickUpAuthenticationType.OAuthAccessToken"/>.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets which configured credential the default client sends.
    /// </summary>
    public ClickUpAuthenticationType AuthenticationType { get; set; } = ClickUpAuthenticationType.PersonalToken;

    /// <summary>
    /// Gets or sets the per-request timeout applied to the shared HTTP client.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

    /// <summary>
    /// Gets or sets bounded retry behavior for rate limits and transient GET failures.
    /// </summary>
    public ClickUpRetryOptions Retry { get; set; } = new();

    internal static Uri CreateBaseUri(string? baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = DefaultBaseUrl;
        }

        if (!baseUrl.EndsWith("/", StringComparison.Ordinal))
        {
            baseUrl += "/";
        }

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
        {
            throw new ArgumentException("ClickUp BaseUrl must be an absolute HTTP or HTTPS URI.", nameof(baseUrl));
        }

        return uri;
    }
}

/// <summary>
/// Bounded retry settings for the ClickUp HTTP pipeline.
/// </summary>
public sealed class ClickUpRetryOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether retries are enabled.
    /// POST, PUT, and DELETE requests are retried only for HTTP 429.
    /// GET requests may also be retried for transient status codes 408, 500, 502, 503, and 504.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of retries after the initial attempt. Valid range is 0 through 5.
    /// </summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>
    /// Gets or sets the maximum delay honored for a single retry.
    /// When ClickUp asks for a longer wait, the request fails instead of sleeping past this limit.
    /// </summary>
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);
}
