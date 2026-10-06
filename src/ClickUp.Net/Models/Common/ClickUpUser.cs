using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp user returned by the API.
/// </summary>
public sealed class ClickUpUser
{
    /// <summary>Gets or sets the user identifier.</summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>Gets or sets the username.</summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>Gets or sets the email address.</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>Gets or sets the profile color.</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>Gets or sets the profile picture URL.</summary>
    [JsonPropertyName("profilePicture")]
    public string? ProfilePicture { get; set; }

    /// <summary>Gets or sets the user's initials.</summary>
    [JsonPropertyName("initials")]
    public string? Initials { get; set; }

    /// <summary>
    /// Gets or sets the preferred first day of the week. <c>0</c> is Sunday and <c>1</c> is Monday.
    /// </summary>
    [JsonPropertyName("week_start_day")]
    public int? WeekStartDay { get; set; }

    /// <summary>Gets or sets a value indicating whether global font support is enabled.</summary>
    [JsonPropertyName("global_font_support")]
    public bool? GlobalFontSupport { get; set; }

    /// <summary>Gets or sets the user's timezone.</summary>
    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
}
