using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp Folder.
/// </summary>
public sealed class ClickUpFolder
{
    /// <summary>Gets or sets the Folder identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the Folder name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the Folder order index.</summary>
    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; set; }

    /// <summary>Gets or sets a value indicating whether the Folder overrides Space statuses.</summary>
    [JsonPropertyName("override_statuses")]
    public bool? OverrideStatuses { get; set; }

    /// <summary>Gets or sets a value indicating whether the Folder is hidden.</summary>
    [JsonPropertyName("hidden")]
    public bool? Hidden { get; set; }

    /// <summary>Gets or sets a value indicating whether the Folder is archived.</summary>
    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    /// <summary>Gets or sets the parent Space.</summary>
    [JsonPropertyName("space")]
    public ClickUpLocation? Space { get; set; }

    /// <summary>Gets or sets the task count reported for the Folder.</summary>
    [JsonPropertyName("task_count")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? TaskCount { get; set; }

    /// <summary>Gets or sets the Folder statuses.</summary>
    [JsonPropertyName("statuses")]
    public IReadOnlyList<ClickUpStatus>? Statuses { get; set; }

    /// <summary>Gets or sets the Lists contained by the Folder when the endpoint includes them.</summary>
    [JsonPropertyName("lists")]
    public IReadOnlyList<ClickUpList>? Lists { get; set; }

    /// <summary>Gets or sets the parent Folder identifier when this Folder is nested.</summary>
    [JsonPropertyName("parent_folder")]
    public string? ParentFolder { get; set; }

    /// <summary>Gets or sets nested Folders when requested.</summary>
    [JsonPropertyName("folders")]
    public IReadOnlyList<ClickUpFolder>? Folders { get; set; }

    /// <summary>Gets or sets the permission level, when returned.</summary>
    [JsonPropertyName("permission_level")]
    public string? PermissionLevel { get; set; }
}

/// <summary>
/// Request body for creating a Folder.
/// </summary>
public sealed class CreateFolderRequest
{
    /// <summary>Gets or sets the Folder name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the parent Folder identifier when creating a nested Folder.</summary>
    [JsonPropertyName("parent_folder_id")]
    public string? ParentFolderId { get; set; }
}

/// <summary>
/// Request body for updating a Folder. Unspecified properties are omitted.
/// </summary>
[JsonConverter(typeof(OptionalPayloadConverter<UpdateFolderRequest>))]
public sealed class UpdateFolderRequest
{
    /// <summary>Gets or sets the Folder name.</summary>
    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }
}

internal sealed class ClickUpFoldersResponse
{
    [JsonPropertyName("folders")]
    public IReadOnlyList<ClickUpFolder>? Folders { get; set; }
}
