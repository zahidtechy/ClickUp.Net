using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// One rich-text fragment inside a comment. Unknown properties are preserved.
/// </summary>
public sealed class ClickUpCommentFragment
{
    /// <summary>Gets or sets the fragment text.</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>Gets or sets properties ClickUp includes beyond the documented text field, such as mention attributes.</summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalProperties { get; set; }
}

/// <summary>
/// A ClickUp comment.
/// </summary>
public sealed class ClickUpComment
{
    /// <summary>Gets or sets the comment identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the rich-text fragments.</summary>
    [JsonPropertyName("comment")]
    public IReadOnlyList<ClickUpCommentFragment>? Comment { get; set; }

    /// <summary>Gets or sets the plain comment text.</summary>
    [JsonPropertyName("comment_text")]
    public string? CommentText { get; set; }

    /// <summary>Gets or sets the author.</summary>
    [JsonPropertyName("user")]
    public ClickUpUser? User { get; set; }

    /// <summary>Gets or sets a value indicating whether the comment is resolved.</summary>
    [JsonPropertyName("resolved")]
    public bool? Resolved { get; set; }

    /// <summary>Gets or sets the assignee.</summary>
    [JsonPropertyName("assignee")]
    public ClickUpUser? Assignee { get; set; }

    /// <summary>Gets or sets the user who assigned the comment.</summary>
    [JsonPropertyName("assigned_by")]
    public ClickUpUser? AssignedBy { get; set; }

    /// <summary>Gets or sets reactions.</summary>
    [JsonPropertyName("reactions")]
    public IReadOnlyList<JsonElement>? Reactions { get; set; }

    /// <summary>Gets or sets the comment time.</summary>
    [JsonPropertyName("date")]
    public DateTimeOffset? Date { get; set; }

    /// <summary>Gets or sets the reply count, when returned.</summary>
    [JsonPropertyName("reply_count")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? ReplyCount { get; set; }
}

/// <summary>Request body for creating a task, list, or chat comment.</summary>
public sealed class CreateCommentRequest
{
    /// <summary>Gets or sets the comment text.</summary>
    [JsonPropertyName("comment_text")]
    public string CommentText { get; set; } = string.Empty;

    /// <summary>Gets or sets the assignee user identifier.</summary>
    [JsonPropertyName("assignee")]
    public int? Assignee { get; set; }

    /// <summary>Gets or sets the group assignee identifier.</summary>
    [JsonPropertyName("group_assignee")]
    public string? GroupAssignee { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the comment creator is notified.
    /// Other assignees and watchers are notified regardless of this setting.
    /// </summary>
    [JsonPropertyName("notify_all")]
    public bool NotifyAll { get; set; }
}

/// <summary>Request body for updating a comment. Unspecified properties are omitted.</summary>
[JsonConverter(typeof(OptionalPayloadConverter<UpdateCommentRequest>))]
public sealed class UpdateCommentRequest
{
    /// <summary>Gets or sets the comment text.</summary>
    [JsonPropertyName("comment_text")]
    public Optional<string?> CommentText { get; set; }

    /// <summary>Gets or sets the assignee user identifier.</summary>
    [JsonPropertyName("assignee")]
    public Optional<int?> Assignee { get; set; }

    /// <summary>Gets or sets the group assignee identifier.</summary>
    [JsonPropertyName("group_assignee")]
    public Optional<int?> GroupAssignee { get; set; }

    /// <summary>Gets or sets a value indicating whether the comment is resolved.</summary>
    [JsonPropertyName("resolved")]
    public Optional<bool?> Resolved { get; set; }
}

/// <summary>The identifier returned after a comment is created.</summary>
public sealed class ClickUpCommentCreated
{
    /// <summary>Gets or sets the comment identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the history identifier.</summary>
    [JsonPropertyName("hist_id")]
    public string? HistoryId { get; set; }

    /// <summary>Gets or sets the comment time.</summary>
    [JsonPropertyName("date")]
    public DateTimeOffset? Date { get; set; }
}

/// <summary>Query options for comment collection endpoints.</summary>
public sealed class GetCommentsRequest
{
    /// <summary>Gets or sets a Unix millisecond timestamp. Comments created before this time are returned.</summary>
    public DateTimeOffset? Start { get; set; }

    /// <summary>Gets or sets a comment identifier to page from.</summary>
    public string? StartId { get; set; }
}

internal sealed class ClickUpCommentsResponse
{
    [JsonPropertyName("comments")]
    public IReadOnlyList<ClickUpComment>? Comments { get; set; }
}
