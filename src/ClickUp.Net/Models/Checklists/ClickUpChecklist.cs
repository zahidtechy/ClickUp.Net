using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A checklist on a task.
/// </summary>
public sealed class ClickUpChecklist
{
    /// <summary>Gets or sets the checklist identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the task identifier.</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>Gets or sets the checklist name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the creation time, when returned.</summary>
    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; set; }

    /// <summary>Gets or sets the order index.</summary>
    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; set; }

    /// <summary>Gets or sets the number of resolved items.</summary>
    [JsonPropertyName("resolved")]
    public int? ResolvedCount { get; set; }

    /// <summary>Gets or sets the number of unresolved items.</summary>
    [JsonPropertyName("unresolved")]
    public int? UnresolvedCount { get; set; }

    /// <summary>Gets or sets the checklist items.</summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<ClickUpChecklistItem>? Items { get; set; }
}

/// <summary>
/// An item in a checklist.
/// </summary>
public sealed class ClickUpChecklistItem
{
    /// <summary>Gets or sets the item identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the item name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the order index.</summary>
    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; set; }

    /// <summary>Gets or sets the assignee.</summary>
    [JsonPropertyName("assignee")]
    public ClickUpUser? Assignee { get; set; }

    /// <summary>Gets or sets a value indicating whether the item is resolved.</summary>
    [JsonPropertyName("resolved")]
    public bool? Resolved { get; set; }

    /// <summary>Gets or sets the parent item identifier.</summary>
    [JsonPropertyName("parent")]
    public string? Parent { get; set; }

    /// <summary>Gets or sets the creation time.</summary>
    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; set; }

    /// <summary>Gets or sets nested checklist items.</summary>
    [JsonPropertyName("children")]
    public IReadOnlyList<ClickUpChecklistItem>? Children { get; set; }
}

/// <summary>Request body for creating a checklist.</summary>
public sealed class CreateChecklistRequest
{
    /// <summary>Gets or sets the checklist name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>Request body for updating a checklist. Unspecified properties are omitted.</summary>
[JsonConverter(typeof(OptionalPayloadConverter<UpdateChecklistRequest>))]
public sealed class UpdateChecklistRequest
{
    /// <summary>Gets or sets the checklist name.</summary>
    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }

    /// <summary>Gets or sets the zero-based position of the checklist on the task.</summary>
    [JsonPropertyName("position")]
    public Optional<int?> Position { get; set; }
}

/// <summary>Request body for creating a checklist item.</summary>
public sealed class CreateChecklistItemRequest
{
    /// <summary>Gets or sets the item name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the assignee user identifier.</summary>
    [JsonPropertyName("assignee")]
    public int? Assignee { get; set; }
}

/// <summary>Request body for updating a checklist item. Unspecified properties are omitted.</summary>
[JsonConverter(typeof(OptionalPayloadConverter<UpdateChecklistItemRequest>))]
public sealed class UpdateChecklistItemRequest
{
    /// <summary>Gets or sets the item name.</summary>
    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }

    /// <summary>Gets or sets the assignee. Send null to clear it.</summary>
    [JsonPropertyName("assignee")]
    public Optional<string?> Assignee { get; set; }

    /// <summary>Gets or sets a value indicating whether the item is resolved.</summary>
    [JsonPropertyName("resolved")]
    public Optional<bool?> Resolved { get; set; }

    /// <summary>Gets or sets the parent item identifier. Send null to move the item to the top level.</summary>
    [JsonPropertyName("parent")]
    public Optional<string?> Parent { get; set; }
}

internal sealed class ClickUpChecklistResponse
{
    [JsonPropertyName("checklist")]
    public ClickUpChecklist? Checklist { get; set; }
}
