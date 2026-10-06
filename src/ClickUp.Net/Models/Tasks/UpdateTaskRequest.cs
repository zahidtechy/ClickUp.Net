using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// Request body for updating a task.
/// Properties left unspecified are omitted. Assign null through <see cref="Optional{T}"/> to send JSON null.
/// </summary>
[JsonConverter(typeof(OptionalPayloadConverter<UpdateTaskRequest>))]
public sealed class UpdateTaskRequest
{
    /// <summary>Gets or sets the custom task type identifier.</summary>
    [JsonPropertyName("custom_item_id")]
    public Optional<long?> CustomItemId { get; set; }

    /// <summary>Gets or sets the task name.</summary>
    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }

    /// <summary>Gets or sets Markdown content.</summary>
    [JsonPropertyName("markdown_content")]
    public Optional<string?> MarkdownContent { get; set; }

    /// <summary>Gets or sets the status name.</summary>
    [JsonPropertyName("status")]
    public Optional<string?> Status { get; set; }

    /// <summary>Gets or sets the priority identifier.</summary>
    [JsonPropertyName("priority")]
    public Optional<int?> Priority { get; set; }

    /// <summary>Gets or sets the due date.</summary>
    [JsonPropertyName("due_date")]
    public Optional<DateTimeOffset?> DueDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the due date includes a time.</summary>
    [JsonPropertyName("due_date_time")]
    public Optional<bool?> DueDateTime { get; set; }

    /// <summary>Gets or sets the parent task identifier.</summary>
    [JsonPropertyName("parent")]
    public Optional<string?> Parent { get; set; }

    /// <summary>Gets or sets the time estimate in milliseconds.</summary>
    [JsonPropertyName("time_estimate")]
    public Optional<int?> TimeEstimate { get; set; }

    /// <summary>Gets or sets the start date.</summary>
    [JsonPropertyName("start_date")]
    public Optional<DateTimeOffset?> StartDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the start date includes a time.</summary>
    [JsonPropertyName("start_date_time")]
    public Optional<bool?> StartDateTime { get; set; }

    /// <summary>Gets or sets sprint points.</summary>
    [JsonPropertyName("points")]
    public Optional<double?> Points { get; set; }

    /// <summary>Gets or sets assignees to add and remove.</summary>
    [JsonPropertyName("assignees")]
    public Optional<ClickUpMemberChange<int>?> Assignees { get; set; }

    /// <summary>Gets or sets group assignees to add and remove.</summary>
    [JsonPropertyName("group_assignees")]
    public Optional<ClickUpMemberChange<string>?> GroupAssignees { get; set; }

    /// <summary>Gets or sets watchers to add and remove.</summary>
    [JsonPropertyName("watchers")]
    public Optional<ClickUpMemberChange<int>?> Watchers { get; set; }

    /// <summary>Gets or sets a value indicating whether the task is archived.</summary>
    [JsonPropertyName("archived")]
    public Optional<bool?> Archived { get; set; }
}
