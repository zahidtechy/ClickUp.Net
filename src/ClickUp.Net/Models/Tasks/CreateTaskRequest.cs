using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// Request body for creating a task in a List.
/// </summary>
public sealed class CreateTaskRequest
{
    /// <summary>Gets or sets the task name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the task description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets Markdown content used as the description.</summary>
    [JsonPropertyName("markdown_content")]
    public string? MarkdownContent { get; set; }

    /// <summary>Gets or sets assignee user identifiers.</summary>
    [JsonPropertyName("assignees")]
    public IReadOnlyList<int>? Assignees { get; set; }

    /// <summary>Gets or sets group assignee identifiers.</summary>
    [JsonPropertyName("group_assignees")]
    public IReadOnlyList<string>? GroupAssignees { get; set; }

    /// <summary>Gets or sets tag names.</summary>
    [JsonPropertyName("tags")]
    public IReadOnlyList<string>? Tags { get; set; }

    /// <summary>Gets or sets the status name.</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>Gets or sets the priority identifier. See <see cref="ClickUpTaskPriorities"/>.</summary>
    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    /// <summary>Gets or sets the due date. The value is sent as Unix milliseconds.</summary>
    [JsonPropertyName("due_date")]
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the due date includes a time.</summary>
    [JsonPropertyName("due_date_time")]
    public bool? DueDateTime { get; set; }

    /// <summary>Gets or sets the start date. The value is sent as Unix milliseconds.</summary>
    [JsonPropertyName("start_date")]
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>Gets or sets a value indicating whether the start date includes a time.</summary>
    [JsonPropertyName("start_date_time")]
    public bool? StartDateTime { get; set; }

    /// <summary>Gets or sets the time estimate in milliseconds.</summary>
    [JsonPropertyName("time_estimate")]
    public int? TimeEstimate { get; set; }

    /// <summary>Gets or sets sprint points.</summary>
    [JsonPropertyName("points")]
    public double? Points { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the task creator is notified.
    /// Assignees and watchers are notified regardless of this setting.
    /// </summary>
    [JsonPropertyName("notify_all")]
    public bool? NotifyAll { get; set; }

    /// <summary>Gets or sets the parent task identifier when creating a subtask.</summary>
    [JsonPropertyName("parent")]
    public string? Parent { get; set; }

    /// <summary>Gets or sets a task identifier this task should link to.</summary>
    [JsonPropertyName("links_to")]
    public string? LinksTo { get; set; }

    /// <summary>Gets or sets a value indicating whether the task is created archived.</summary>
    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    /// <summary>Gets or sets a value indicating whether required custom fields must be present.</summary>
    [JsonPropertyName("check_required_custom_fields")]
    public bool? CheckRequiredCustomFields { get; set; }

    /// <summary>Gets or sets the custom task type identifier.</summary>
    [JsonPropertyName("custom_item_id")]
    public long? CustomItemId { get; set; }

    /// <summary>Gets or sets custom field values to store on the new task.</summary>
    [JsonPropertyName("custom_fields")]
    public IReadOnlyList<ClickUpTaskCustomField>? CustomFields { get; set; }
}
