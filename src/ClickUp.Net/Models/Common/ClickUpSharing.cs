using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// Public sharing settings returned on a task.
/// </summary>
public sealed class ClickUpSharing
{
    /// <summary>Gets or sets a value indicating whether the task is shared publicly.</summary>
    [JsonPropertyName("public")]
    public bool? Public { get; set; }

    /// <summary>Gets or sets the public share expiration time.</summary>
    [JsonPropertyName("public_share_expires_on")]
    public DateTimeOffset? PublicShareExpiresOn { get; set; }

    /// <summary>Gets or sets the fields included in the public share.</summary>
    [JsonPropertyName("public_fields")]
    public IReadOnlyList<string>? PublicFields { get; set; }

    /// <summary>Gets or sets the public share token. Do not log this value.</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>Gets or sets a value indicating whether the public share is SEO optimized.</summary>
    [JsonPropertyName("seo_optimized")]
    public bool? SeoOptimized { get; set; }
}
