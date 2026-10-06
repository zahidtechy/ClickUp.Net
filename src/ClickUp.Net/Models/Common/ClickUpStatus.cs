using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp task or list status.
/// </summary>
public sealed class ClickUpStatus
{
    /// <summary>Gets or sets the status identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the status name.</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>Gets or sets the status color.</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>Gets or sets the status order index.</summary>
    [JsonPropertyName("orderindex")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? OrderIndex { get; set; }

    /// <summary>Gets or sets the status type, such as <c>open</c>, <c>custom</c>, <c>done</c>, or <c>closed</c>.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Gets or sets a value indicating whether the status label is hidden.</summary>
    [JsonPropertyName("hide_label")]
    public bool? HideLabel { get; set; }
}
