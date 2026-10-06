using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp Space.
/// </summary>
public sealed class ClickUpSpace
{
    /// <summary>Gets or sets the Space identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the Space name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets a value indicating whether the Space is private.</summary>
    [JsonPropertyName("private")]
    public bool? Private { get; set; }

    /// <summary>Gets or sets the Space color.</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>Gets or sets the Space avatar.</summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>Gets or sets a value indicating whether administrators can manage the Space.</summary>
    [JsonPropertyName("admin_can_manage")]
    public bool? AdminCanManage { get; set; }

    /// <summary>Gets or sets a value indicating whether the Space is archived.</summary>
    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    /// <summary>Gets or sets the Space statuses.</summary>
    [JsonPropertyName("statuses")]
    public IReadOnlyList<ClickUpStatus>? Statuses { get; set; }

    /// <summary>Gets or sets a value indicating whether tasks can have multiple assignees.</summary>
    [JsonPropertyName("multiple_assignees")]
    public bool? MultipleAssignees { get; set; }

    /// <summary>Gets or sets the Space feature configuration.</summary>
    [JsonPropertyName("features")]
    public ClickUpSpaceFeatures? Features { get; set; }
}

/// <summary>
/// Request body for creating a Space.
/// </summary>
public sealed class CreateSpaceRequest
{
    /// <summary>Gets or sets the Space name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether tasks in the Space allow multiple assignees.</summary>
    [JsonPropertyName("multiple_assignees")]
    public bool MultipleAssignees { get; set; }

    /// <summary>Gets or sets the feature configuration ClickUp requires when creating a Space.</summary>
    [JsonPropertyName("features")]
    public ClickUpSpaceFeatures? Features { get; set; }
}

/// <summary>
/// Request body for updating a Space. Unspecified properties are omitted. Explicit null is sent as JSON null.
/// </summary>
[JsonConverter(typeof(OptionalPayloadConverter<UpdateSpaceRequest>))]
public sealed class UpdateSpaceRequest
{
    /// <summary>Gets or sets the Space name.</summary>
    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }

    /// <summary>Gets or sets the Space color.</summary>
    [JsonPropertyName("color")]
    public Optional<string?> Color { get; set; }

    /// <summary>Gets or sets a value indicating whether the Space is private.</summary>
    [JsonPropertyName("private")]
    public Optional<bool?> Private { get; set; }

    /// <summary>Gets or sets a value indicating whether administrators can manage the Space.</summary>
    [JsonPropertyName("admin_can_manage")]
    public Optional<bool?> AdminCanManage { get; set; }

    /// <summary>Gets or sets a value indicating whether multiple assignees are enabled.</summary>
    [JsonPropertyName("multiple_assignees")]
    public Optional<bool?> MultipleAssignees { get; set; }

    /// <summary>Gets or sets the feature configuration.</summary>
    [JsonPropertyName("features")]
    public Optional<ClickUpSpaceFeatures?> Features { get; set; }
}

internal sealed class ClickUpSpacesResponse
{
    [JsonPropertyName("spaces")]
    public IReadOnlyList<ClickUpSpace>? Spaces { get; set; }
}
