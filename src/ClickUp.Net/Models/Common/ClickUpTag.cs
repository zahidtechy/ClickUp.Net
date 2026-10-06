using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp tag.
/// </summary>
public sealed class ClickUpTag
{
    /// <summary>Gets or sets the tag name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the foreground color.</summary>
    [JsonPropertyName("tag_fg")]
    public string? ForegroundColor { get; set; }

    /// <summary>Gets or sets the background color.</summary>
    [JsonPropertyName("tag_bg")]
    public string? BackgroundColor { get; set; }

    /// <summary>Gets or sets the identifier of the user who created the tag, when returned.</summary>
    [JsonPropertyName("creator")]
    public int? Creator { get; set; }
}
