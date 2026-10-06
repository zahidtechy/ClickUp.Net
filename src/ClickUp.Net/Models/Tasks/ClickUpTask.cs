using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A user or group membership change sent when updating a task.
/// </summary>
public sealed class ClickUpMemberChange<T>
{
    /// <summary>Gets or sets the identifiers to add.</summary>
    [JsonPropertyName("add")]
    public IReadOnlyList<T> Add { get; set; } = Array.Empty<T>();

    /// <summary>Gets or sets the identifiers to remove.</summary>
    [JsonPropertyName("rem")]
    public IReadOnlyList<T> Remove { get; set; } = Array.Empty<T>();
}

/// <summary>
/// A task dependency returned by ClickUp.
/// </summary>
public sealed class ClickUpTaskDependency
{
    /// <summary>Gets or sets the task identifier.</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>Gets or sets the task this task depends on.</summary>
    [JsonPropertyName("depends_on")]
    public string? DependsOn { get; set; }

    /// <summary>Gets or sets the dependency type.</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>Gets or sets the time the dependency was created.</summary>
    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; set; }

    /// <summary>Gets or sets the user identifier that created the dependency.</summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>Gets or sets the Workspace identifier.</summary>
    [JsonPropertyName("workspace_id")]
    public string? WorkspaceId { get; set; }
}

/// <summary>
/// A file attached to a task.
/// </summary>
public sealed class ClickUpAttachment
{
    /// <summary>Gets or sets the attachment identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the attachment date.</summary>
    [JsonPropertyName("date")]
    public DateTimeOffset? Date { get; set; }

    /// <summary>Gets or sets the title.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Gets or sets the attachment type.</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>Gets or sets the source.</summary>
    [JsonPropertyName("source")]
    public int? Source { get; set; }

    /// <summary>Gets or sets the version.</summary>
    [JsonPropertyName("version")]
    public int? Version { get; set; }

    /// <summary>Gets or sets the file extension.</summary>
    [JsonPropertyName("extension")]
    public string? Extension { get; set; }

    /// <summary>Gets or sets the MIME type.</summary>
    [JsonPropertyName("mimetype")]
    public string? MimeType { get; set; }

    /// <summary>Gets or sets the file size in bytes.</summary>
    [JsonPropertyName("size")]
    public long? Size { get; set; }

    /// <summary>Gets or sets the download URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Gets or sets the URL including the host.</summary>
    [JsonPropertyName("url_w_host")]
    public string? UrlWithHost { get; set; }

    /// <summary>Gets or sets the user who uploaded the attachment.</summary>
    [JsonPropertyName("user")]
    public ClickUpUser? User { get; set; }

    /// <summary>Gets or sets a value indicating whether the attachment is deleted.</summary>
    [JsonPropertyName("deleted")]
    public bool? Deleted { get; set; }

    /// <summary>Gets or sets a value indicating whether the attachment is hidden.</summary>
    [JsonPropertyName("hidden")]
    public bool? Hidden { get; set; }
}

/// <summary>
/// A ClickUp task.
/// </summary>
public sealed class ClickUpTask
{
    /// <summary>Gets or sets the task identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the custom task identifier.</summary>
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }

    /// <summary>Gets or sets the custom task type identifier.</summary>
    [JsonPropertyName("custom_item_id")]
    public long? CustomItemId { get; set; }

    /// <summary>Gets or sets the task name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the plain text content.</summary>
    [JsonPropertyName("text_content")]
    public string? TextContent { get; set; }

    /// <summary>Gets or sets the task description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets the Markdown description when it was requested.</summary>
    [JsonPropertyName("markdown_description")]
    public string? MarkdownDescription { get; set; }

    /// <summary>Gets or sets the task status.</summary>
    [JsonPropertyName("status")]
    public ClickUpStatus? Status { get; set; }

    /// <summary>Gets or sets the order index.</summary>
    [JsonPropertyName("orderindex")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? OrderIndex { get; set; }

    /// <summary>Gets or sets the creation time.</summary>
    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; set; }

    /// <summary>Gets or sets the last update time.</summary>
    [JsonPropertyName("date_updated")]
    public DateTimeOffset? DateUpdated { get; set; }

    /// <summary>Gets or sets the closed time.</summary>
    [JsonPropertyName("date_closed")]
    public DateTimeOffset? DateClosed { get; set; }

    /// <summary>Gets or sets the done time.</summary>
    [JsonPropertyName("date_done")]
    public DateTimeOffset? DateDone { get; set; }

    /// <summary>Gets or sets a value indicating whether the task is archived.</summary>
    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    /// <summary>Gets or sets the user who created the task.</summary>
    [JsonPropertyName("creator")]
    public ClickUpUser? Creator { get; set; }

    /// <summary>Gets or sets the assignees.</summary>
    [JsonPropertyName("assignees")]
    public IReadOnlyList<ClickUpUser>? Assignees { get; set; }

    /// <summary>Gets or sets group assignees. The OpenAPI schema does not fully describe this object.</summary>
    [JsonPropertyName("group_assignees")]
    public IReadOnlyList<JsonElement>? GroupAssignees { get; set; }

    /// <summary>Gets or sets the watchers.</summary>
    [JsonPropertyName("watchers")]
    public IReadOnlyList<ClickUpUser>? Watchers { get; set; }

    /// <summary>Gets or sets the checklists.</summary>
    [JsonPropertyName("checklists")]
    public IReadOnlyList<ClickUpChecklist>? Checklists { get; set; }

    /// <summary>Gets or sets the tags.</summary>
    [JsonPropertyName("tags")]
    public IReadOnlyList<ClickUpTag>? Tags { get; set; }

    /// <summary>Gets or sets the parent task identifier.</summary>
    [JsonPropertyName("parent")]
    public string? Parent { get; set; }

    /// <summary>Gets or sets the top-level parent task identifier.</summary>
    [JsonPropertyName("top_level_parent")]
    public string? TopLevelParent { get; set; }

    /// <summary>Gets or sets the priority.</summary>
    [JsonPropertyName("priority")]
    public ClickUpPriority? Priority { get; set; }

    /// <summary>Gets or sets the due date.</summary>
    [JsonPropertyName("due_date")]
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>Gets or sets the start date.</summary>
    [JsonPropertyName("start_date")]
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>Gets or sets sprint points.</summary>
    [JsonPropertyName("points")]
    public double? Points { get; set; }

    /// <summary>Gets or sets the time estimate in milliseconds.</summary>
    [JsonPropertyName("time_estimate")]
    public long? TimeEstimate { get; set; }

    /// <summary>Gets or sets tracked time in milliseconds.</summary>
    [JsonPropertyName("time_spent")]
    public long? TimeSpent { get; set; }

    /// <summary>Gets or sets custom fields that apply to this task.</summary>
    [JsonPropertyName("custom_fields")]
    public IReadOnlyList<ClickUpCustomField>? CustomFields { get; set; }

    /// <summary>Gets or sets task dependencies.</summary>
    [JsonPropertyName("dependencies")]
    public IReadOnlyList<ClickUpTaskDependency>? Dependencies { get; set; }

    /// <summary>Gets or sets linked tasks. The OpenAPI schema does not describe the item shape.</summary>
    [JsonPropertyName("linked_tasks")]
    public IReadOnlyList<JsonElement>? LinkedTasks { get; set; }

    /// <summary>Gets or sets additional task locations.</summary>
    [JsonPropertyName("locations")]
    public IReadOnlyList<ClickUpLocation>? Locations { get; set; }

    /// <summary>Gets or sets the Workspace identifier.</summary>
    [JsonPropertyName("team_id")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? TeamId { get; set; }

    /// <summary>Gets or sets the task URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Gets or sets public sharing settings.</summary>
    [JsonPropertyName("sharing")]
    public ClickUpSharing? Sharing { get; set; }

    /// <summary>Gets or sets the permission level.</summary>
    [JsonPropertyName("permission_level")]
    public string? PermissionLevel { get; set; }

    /// <summary>Gets or sets the home List.</summary>
    [JsonPropertyName("list")]
    public ClickUpLocation? List { get; set; }

    /// <summary>Gets or sets the parent Folder. ClickUp also returns this object as <c>project</c>.</summary>
    [JsonPropertyName("folder")]
    public ClickUpLocation? Folder { get; set; }

    /// <summary>Gets or sets the parent Folder when ClickUp returns the legacy <c>project</c> property.</summary>
    [JsonPropertyName("project")]
    public ClickUpLocation? Project { get; set; }

    /// <summary>Gets or sets the parent Space.</summary>
    [JsonPropertyName("space")]
    public ClickUpLocation? Space { get; set; }

    /// <summary>Gets or sets subtasks when they were included in the request.</summary>
    [JsonPropertyName("subtasks")]
    public IReadOnlyList<ClickUpTask>? Subtasks { get; set; }

    /// <summary>Gets or sets attachments.</summary>
    [JsonPropertyName("attachments")]
    public IReadOnlyList<ClickUpAttachment>? Attachments { get; set; }
}
