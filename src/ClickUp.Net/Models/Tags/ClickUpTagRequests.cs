using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>Request body for creating or replacing a Space tag.</summary>
public sealed class ClickUpTagRequest
{
    /// <summary>Gets or sets the tag.</summary>
    [JsonPropertyName("tag")]
    public ClickUpTag Tag { get; set; } = new();
}

internal sealed class ClickUpTagsResponse
{
    [JsonPropertyName("tags")]
    public IReadOnlyList<ClickUpTag>? Tags { get; set; }
}
