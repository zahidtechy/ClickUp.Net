using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// Event names accepted by the ClickUp webhook endpoints. <see cref="All"/> subscribes to every event.
/// </summary>
public static class ClickUpWebhookEvents
{
    /// <summary>Subscribe to every event.</summary>
    public const string All = "*";

    /// <summary>A task was created.</summary>
    public const string TaskCreated = "taskCreated";

    /// <summary>A task was updated.</summary>
    public const string TaskUpdated = "taskUpdated";

    /// <summary>A task was deleted.</summary>
    public const string TaskDeleted = "taskDeleted";

    /// <summary>A task priority changed.</summary>
    public const string TaskPriorityUpdated = "taskPriorityUpdated";

    /// <summary>A task status changed.</summary>
    public const string TaskStatusUpdated = "taskStatusUpdated";

    /// <summary>A task assignee changed.</summary>
    public const string TaskAssigneeUpdated = "taskAssigneeUpdated";

    /// <summary>A task due date changed.</summary>
    public const string TaskDueDateUpdated = "taskDueDateUpdated";

    /// <summary>A task tag changed.</summary>
    public const string TaskTagUpdated = "taskTagUpdated";

    /// <summary>A task moved.</summary>
    public const string TaskMoved = "taskMoved";

    /// <summary>A task comment was posted.</summary>
    public const string TaskCommentPosted = "taskCommentPosted";

    /// <summary>A task comment was updated.</summary>
    public const string TaskCommentUpdated = "taskCommentUpdated";

    /// <summary>A task time estimate changed.</summary>
    public const string TaskTimeEstimateUpdated = "taskTimeEstimateUpdated";

    /// <summary>Tracked time on a task changed.</summary>
    public const string TaskTimeTrackedUpdated = "taskTimeTrackedUpdated";

    /// <summary>A List was created.</summary>
    public const string ListCreated = "listCreated";

    /// <summary>A List was updated.</summary>
    public const string ListUpdated = "listUpdated";

    /// <summary>A List was deleted.</summary>
    public const string ListDeleted = "listDeleted";

    /// <summary>A Folder was created.</summary>
    public const string FolderCreated = "folderCreated";

    /// <summary>A Folder was updated.</summary>
    public const string FolderUpdated = "folderUpdated";

    /// <summary>A Folder was deleted.</summary>
    public const string FolderDeleted = "folderDeleted";

    /// <summary>A Space was created.</summary>
    public const string SpaceCreated = "spaceCreated";

    /// <summary>A Space was updated.</summary>
    public const string SpaceUpdated = "spaceUpdated";

    /// <summary>A Space was deleted.</summary>
    public const string SpaceDeleted = "spaceDeleted";

    /// <summary>A Goal was created.</summary>
    public const string GoalCreated = "goalCreated";

    /// <summary>A Goal was updated.</summary>
    public const string GoalUpdated = "goalUpdated";

    /// <summary>A Goal was deleted.</summary>
    public const string GoalDeleted = "goalDeleted";

    /// <summary>A key result was created.</summary>
    public const string KeyResultCreated = "keyResultCreated";

    /// <summary>A key result was updated.</summary>
    public const string KeyResultUpdated = "keyResultUpdated";

    /// <summary>A key result was deleted.</summary>
    public const string KeyResultDeleted = "keyResultDeleted";
}

/// <summary>
/// Health reported for a webhook registration.
/// </summary>
public sealed class ClickUpWebhookHealth
{
    /// <summary>Gets or sets the health status, such as <c>active</c> or <c>failing</c>.</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>Gets or sets the failure count.</summary>
    [JsonPropertyName("fail_count")]
    public int? FailCount { get; set; }
}

/// <summary>
/// A ClickUp webhook registration. <see cref="Secret"/> is returned by ClickUp and must not be logged.
/// </summary>
public sealed class ClickUpWebhook
{
    /// <summary>Gets or sets the webhook identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the user identifier that owns the webhook.</summary>
    [JsonPropertyName("userid")]
    public int? UserId { get; set; }

    /// <summary>Gets or sets the Workspace identifier.</summary>
    [JsonPropertyName("team_id")]
    public long? TeamId { get; set; }

    /// <summary>Gets or sets the destination URL.</summary>
    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }

    /// <summary>Gets or sets the OAuth client identifier, when the webhook was created with OAuth.</summary>
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    /// <summary>Gets or sets the subscribed events.</summary>
    [JsonPropertyName("events")]
    public IReadOnlyList<string>? Events { get; set; }

    /// <summary>Gets or sets the task filter.</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>Gets or sets the List filter.</summary>
    [JsonPropertyName("list_id")]
    public string? ListId { get; set; }

    /// <summary>Gets or sets the Folder filter.</summary>
    [JsonPropertyName("folder_id")]
    public string? FolderId { get; set; }

    /// <summary>Gets or sets the Space filter.</summary>
    [JsonPropertyName("space_id")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets webhook health.</summary>
    [JsonPropertyName("health")]
    public ClickUpWebhookHealth? Health { get; set; }

    /// <summary>Gets or sets the signing secret. Do not log or expose this value.</summary>
    [JsonPropertyName("secret")]
    public string? Secret { get; set; }
}

/// <summary>Request body for creating a webhook.</summary>
public sealed class CreateWebhookRequest
{
    /// <summary>Gets or sets the HTTPS endpoint that receives events.</summary>
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>Gets or sets the events to subscribe to. Use <see cref="ClickUpWebhookEvents.All"/> for every event.</summary>
    [JsonPropertyName("events")]
    public IReadOnlyList<string> Events { get; set; } = Array.Empty<string>();

    /// <summary>Gets or sets an optional Space filter.</summary>
    [JsonPropertyName("space_id")]
    public long? SpaceId { get; set; }

    /// <summary>Gets or sets an optional Folder filter.</summary>
    [JsonPropertyName("folder_id")]
    public long? FolderId { get; set; }

    /// <summary>Gets or sets an optional List filter.</summary>
    [JsonPropertyName("list_id")]
    public long? ListId { get; set; }

    /// <summary>Gets or sets an optional task filter.</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }
}

/// <summary>
/// Request body for updating a webhook.
/// The current ClickUp OpenAPI schema types <c>events</c> as a string. <c>*</c> subscribes to every event.
/// </summary>
[JsonConverter(typeof(OptionalPayloadConverter<UpdateWebhookRequest>))]
public sealed class UpdateWebhookRequest
{
    /// <summary>Gets or sets the destination URL.</summary>
    [JsonPropertyName("endpoint")]
    public Optional<string?> Endpoint { get; set; }

    /// <summary>Gets or sets the event subscription. The documented update body sends this as a string.</summary>
    [JsonPropertyName("events")]
    public Optional<string?> Events { get; set; }

    /// <summary>Gets or sets the webhook status.</summary>
    [JsonPropertyName("status")]
    public Optional<string?> Status { get; set; }
}

/// <summary>
/// Common envelope for an incoming ClickUp webhook. Resource identifiers vary by event.
/// </summary>
public class ClickUpWebhookPayload
{
    /// <summary>Gets or sets the event name.</summary>
    [JsonPropertyName("event")]
    public string? Event { get; set; }

    /// <summary>Gets or sets the webhook identifier.</summary>
    [JsonPropertyName("webhook_id")]
    public string? WebhookId { get; set; }

    /// <summary>Gets or sets the task identifier, when the event is about a task.</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>Gets or sets the List identifier, when the event is about a List.</summary>
    [JsonPropertyName("list_id")]
    public string? ListId { get; set; }

    /// <summary>Gets or sets the Folder identifier, when the event is about a Folder.</summary>
    [JsonPropertyName("folder_id")]
    public string? FolderId { get; set; }

    /// <summary>Gets or sets the Space identifier, when the event is about a Space.</summary>
    [JsonPropertyName("space_id")]
    public string? SpaceId { get; set; }

    /// <summary>Gets or sets the Goal identifier, when the event is about a Goal.</summary>
    [JsonPropertyName("goal_id")]
    public string? GoalId { get; set; }

    /// <summary>Gets or sets history items that describe the change.</summary>
    [JsonPropertyName("history_items")]
    public IReadOnlyList<ClickUpWebhookHistoryItem>? HistoryItems { get; set; }
}

/// <summary>
/// One history item inside a webhook payload. <see cref="Before"/> and <see cref="After"/> keep their original JSON shape.
/// </summary>
public sealed class ClickUpWebhookHistoryItem
{
    /// <summary>Gets or sets the history item identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the history item type.</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>Gets or sets the history item time.</summary>
    [JsonPropertyName("date")]
    public DateTimeOffset? Date { get; set; }

    /// <summary>Gets or sets the field that changed, when present.</summary>
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    /// <summary>Gets or sets the user who made the change.</summary>
    [JsonPropertyName("user")]
    public ClickUpUser? User { get; set; }

    /// <summary>Gets or sets the previous value.</summary>
    [JsonPropertyName("before")]
    public JsonElement? Before { get; set; }

    /// <summary>Gets or sets the next value.</summary>
    [JsonPropertyName("after")]
    public JsonElement? After { get; set; }

    /// <summary>Gets or sets the parent comment identifier for threaded comment events, when present.</summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    /// <summary>Gets or sets additional history data whose shape depends on the event.</summary>
    [JsonPropertyName("data")]
    public JsonElement? Data { get; set; }
}

internal sealed class ClickUpWebhooksResponse
{
    [JsonPropertyName("webhooks")]
    public IReadOnlyList<ClickUpWebhook>? Webhooks { get; set; }
}

internal sealed class ClickUpWebhookResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("webhook")]
    public ClickUpWebhook? Webhook { get; set; }
}
