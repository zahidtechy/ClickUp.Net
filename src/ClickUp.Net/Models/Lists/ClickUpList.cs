using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp List.
/// </summary>
public sealed class ClickUpList
{
    /// <summary>Gets or sets the List identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the List name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the List order index.</summary>
    [JsonPropertyName("orderindex")]
    public int? OrderIndex { get; set; }

    /// <summary>Gets or sets the List content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>Gets or sets the List status.</summary>
    [JsonPropertyName("status")]
    public ClickUpStatus? Status { get; set; }

    /// <summary>Gets or sets the List priority.</summary>
    [JsonPropertyName("priority")]
    public ClickUpPriority? Priority { get; set; }

    /// <summary>Gets or sets the List assignee. ClickUp may return an object or null.</summary>
    [JsonPropertyName("assignee")]
    [JsonConverter(typeof(ClickUpUserJsonConverter))]
    public ClickUpUser? Assignee { get; set; }

    /// <summary>Gets or sets the task count.</summary>
    [JsonPropertyName("task_count")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? TaskCount { get; set; }

    /// <summary>Gets or sets the List due date.</summary>
    [JsonPropertyName("due_date")]
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the due date includes a time.</summary>
    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; set; }

    /// <summary>Gets or sets the List start date.</summary>
    [JsonPropertyName("start_date")]
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the start date includes a time.</summary>
    [JsonPropertyName("start_date_time")]
    public bool? StartDateTime { get; set; }

    /// <summary>Gets or sets the parent Folder.</summary>
    [JsonPropertyName("folder")]
    public ClickUpLocation? Folder { get; set; }

    /// <summary>Gets or sets the parent Space.</summary>
    [JsonPropertyName("space")]
    public ClickUpLocation? Space { get; set; }

    /// <summary>Gets or sets a value indicating whether the List is archived.</summary>
    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    /// <summary>Gets or sets a value indicating whether the List overrides inherited statuses.</summary>
    [JsonPropertyName("override_statuses")]
    public bool? OverrideStatuses { get; set; }

    /// <summary>Gets or sets the List statuses.</summary>
    [JsonPropertyName("statuses")]
    public IReadOnlyList<ClickUpStatus>? Statuses { get; set; }

    /// <summary>Gets or sets the inbound email address for the List.</summary>
    [JsonPropertyName("inbound_address")]
    public string? InboundAddress { get; set; }

    /// <summary>Gets or sets the permission level, when returned.</summary>
    [JsonPropertyName("permission_level")]
    public string? PermissionLevel { get; set; }
}

/// <summary>
/// Request body for creating a List.
/// </summary>
public sealed class CreateListRequest
{
    /// <summary>Gets or sets the List name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the List content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>Gets or sets Markdown content. When supplied, ClickUp uses it for the List description.</summary>
    [JsonPropertyName("markdown_content")]
    public string? MarkdownContent { get; set; }

    /// <summary>Gets or sets the due date.</summary>
    [JsonPropertyName("due_date")]
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the due date includes a time.</summary>
    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; set; }

    /// <summary>Gets or sets the priority identifier. See <see cref="ClickUpTaskPriorities"/>.</summary>
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    /// <summary>Gets or sets the assignee user identifier.</summary>
    [JsonPropertyName("assignee")]
    public int? Assignee { get; set; }

    /// <summary>Gets or sets the status name.</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

/// <summary>
/// Request body for updating a List. Unspecified properties are omitted.
/// </summary>
[JsonConverter(typeof(OptionalPayloadConverter<UpdateListRequest>))]
public sealed class UpdateListRequest
{
    /// <summary>Gets or sets the List name.</summary>
    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }

    /// <summary>Gets or sets the List content.</summary>
    [JsonPropertyName("content")]
    public Optional<string?> Content { get; set; }

    /// <summary>Gets or sets Markdown content.</summary>
    [JsonPropertyName("markdown_content")]
    public Optional<string?> MarkdownContent { get; set; }

    /// <summary>Gets or sets the due date.</summary>
    [JsonPropertyName("due_date")]
    public Optional<DateTimeOffset?> DueDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the due date includes a time.</summary>
    [JsonPropertyName("due_date_time")]
    public Optional<bool?> DueDateTime { get; set; }

    /// <summary>Gets or sets the priority identifier.</summary>
    [JsonPropertyName("priority")]
    public Optional<int?> Priority { get; set; }

    /// <summary>Gets or sets the assignee user identifier.</summary>
    [JsonPropertyName("assignee")]
    public Optional<int?> Assignee { get; set; }

    /// <summary>Gets or sets the status name.</summary>
    [JsonPropertyName("status")]
    public Optional<string?> Status { get; set; }

    /// <summary>Gets or sets a value indicating whether the List status should be cleared.</summary>
    [JsonPropertyName("unset_status")]
    public Optional<bool?> UnsetStatus { get; set; }
}

internal sealed class ClickUpListsResponse
{
    [JsonPropertyName("lists")]
    public IReadOnlyList<ClickUpList>? Lists { get; set; }
}
