using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp view. Configuration objects that the OpenAPI schema leaves loosely typed are preserved as JSON.
/// </summary>
public sealed class ClickUpView
{
    /// <summary>Gets or sets the view identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the view name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the view type, such as <c>list</c> or <c>form</c>.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Gets or sets the parent location.</summary>
    [JsonPropertyName("parent")]
    public ClickUpViewParent? Parent { get; set; }

    /// <summary>Gets or sets the creation time.</summary>
    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; set; }

    /// <summary>Gets or sets the creator user identifier.</summary>
    [JsonPropertyName("creator")]
    public int? Creator { get; set; }

    /// <summary>Gets or sets the visibility.</summary>
    [JsonPropertyName("visibility")]
    public string? Visibility { get; set; }

    /// <summary>Gets or sets a value indicating whether the view is protected.</summary>
    [JsonPropertyName("protected")]
    public bool? Protected { get; set; }

    /// <summary>Gets or sets a value indicating whether the view is public.</summary>
    [JsonPropertyName("public")]
    public bool? Public { get; set; }

    /// <summary>Gets or sets the public URL.</summary>
    [JsonPropertyName("public_url")]
    public string? PublicUrl { get; set; }

    /// <summary>Gets or sets the order index.</summary>
    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; set; }

    /// <summary>Gets or sets grouping configuration.</summary>
    [JsonPropertyName("grouping")]
    public JsonElement? Grouping { get; set; }

    /// <summary>Gets or sets divide configuration.</summary>
    [JsonPropertyName("divide")]
    public JsonElement? Divide { get; set; }

    /// <summary>Gets or sets sorting configuration.</summary>
    [JsonPropertyName("sorting")]
    public JsonElement? Sorting { get; set; }

    /// <summary>Gets or sets filter configuration.</summary>
    [JsonPropertyName("filters")]
    public JsonElement? Filters { get; set; }

    /// <summary>Gets or sets column configuration.</summary>
    [JsonPropertyName("columns")]
    public JsonElement? Columns { get; set; }

    /// <summary>Gets or sets view settings.</summary>
    [JsonPropertyName("settings")]
    public JsonElement? Settings { get; set; }
}

/// <summary>The parent of a view.</summary>
public sealed class ClickUpViewParent
{
    /// <summary>Gets or sets the parent identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the parent type.</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }
}

/// <summary>
/// Request body for creating a view. Nested configuration is sent as JSON so new ClickUp view fields are not dropped.
/// </summary>
public sealed class CreateViewRequest
{
    /// <summary>Gets or sets the view name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the view type.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets grouping configuration.</summary>
    [JsonPropertyName("grouping")]
    public JsonElement? Grouping { get; set; }

    /// <summary>Gets or sets divide configuration.</summary>
    [JsonPropertyName("divide")]
    public JsonElement? Divide { get; set; }

    /// <summary>Gets or sets sorting configuration.</summary>
    [JsonPropertyName("sorting")]
    public JsonElement? Sorting { get; set; }

    /// <summary>Gets or sets filter configuration.</summary>
    [JsonPropertyName("filters")]
    public JsonElement? Filters { get; set; }

    /// <summary>Gets or sets column configuration.</summary>
    [JsonPropertyName("columns")]
    public JsonElement? Columns { get; set; }

    /// <summary>Gets or sets team sidebar configuration.</summary>
    [JsonPropertyName("team_sidebar")]
    public JsonElement? TeamSidebar { get; set; }

    /// <summary>Gets or sets view settings.</summary>
    [JsonPropertyName("settings")]
    public JsonElement? Settings { get; set; }
}

internal sealed class ClickUpViewsResponse
{
    [JsonPropertyName("views")]
    public IReadOnlyList<ClickUpView>? Views { get; set; }
}

internal sealed class ClickUpViewResponse
{
    [JsonPropertyName("view")]
    public ClickUpView? View { get; set; }
}
