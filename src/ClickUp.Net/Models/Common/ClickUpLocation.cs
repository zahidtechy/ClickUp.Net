using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A reference to a Space, Folder, List, or other hierarchy location.
/// </summary>
public sealed class ClickUpLocation
{
    /// <summary>Gets or sets the location identifier.</summary>
    [JsonPropertyName("id")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? Id { get; set; }

    /// <summary>Gets or sets the location name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets a value indicating whether the location is hidden.</summary>
    [JsonPropertyName("hidden")]
    public bool? Hidden { get; set; }

    /// <summary>Gets or sets a value indicating whether the current user can access the location.</summary>
    [JsonPropertyName("access")]
    public bool? Access { get; set; }
}
