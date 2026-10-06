using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A task summary included on a time entry.
/// </summary>
public sealed class ClickUpTimeEntryTask
{
    /// <summary>Gets or sets the task identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the custom task identifier.</summary>
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }

    /// <summary>Gets or sets the task name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the task status.</summary>
    [JsonPropertyName("status")]
    public ClickUpStatus? Status { get; set; }

    /// <summary>Gets or sets the custom task type.</summary>
    [JsonPropertyName("custom_type")]
    public string? CustomType { get; set; }
}

/// <summary>
/// A time entry returned by the Workspace time entry endpoints.
/// </summary>
public sealed class ClickUpTimeEntry
{
    /// <summary>Gets or sets the time entry identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the related task.</summary>
    [JsonPropertyName("task")]
    public ClickUpTimeEntryTask? Task { get; set; }

    /// <summary>Gets or sets the Workspace identifier.</summary>
    [JsonPropertyName("wid")]
    public string? WorkspaceId { get; set; }

    /// <summary>Gets or sets the user who tracked the time.</summary>
    [JsonPropertyName("user")]
    public ClickUpUser? User { get; set; }

    /// <summary>Gets or sets a value indicating whether the time is billable.</summary>
    [JsonPropertyName("billable")]
    public bool? Billable { get; set; }

    /// <summary>Gets or sets the start time.</summary>
    [JsonPropertyName("start")]
    public DateTimeOffset? Start { get; set; }

    /// <summary>Gets or sets the end time.</summary>
    [JsonPropertyName("end")]
    public DateTimeOffset? End { get; set; }

    /// <summary>Gets or sets the duration in milliseconds.</summary>
    [JsonPropertyName("duration")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? Duration { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets tags. The list endpoint documents these as strings.</summary>
    [JsonPropertyName("tags")]
    public IReadOnlyList<JsonElement>? Tags { get; set; }

    /// <summary>Gets or sets the source of the entry.</summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>Gets or sets the entry timestamp.</summary>
    [JsonPropertyName("at")]
    public DateTimeOffset? At { get; set; }

    /// <summary>Gets or sets the task URL, when requested.</summary>
    [JsonPropertyName("task_url")]
    public string? TaskUrl { get; set; }

    /// <summary>Gets or sets the approval identifier, when present.</summary>
    [JsonPropertyName("approval_id")]
    public string? ApprovalId { get; set; }
}

/// <summary>A tag sent when creating a time entry.</summary>
public sealed class ClickUpTimeEntryTag
{
    /// <summary>Gets or sets the tag name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the foreground color.</summary>
    [JsonPropertyName("tag_fg")]
    public string? ForegroundColor { get; set; }

    /// <summary>Gets or sets the background color.</summary>
    [JsonPropertyName("tag_bg")]
    public string? BackgroundColor { get; set; }
}

/// <summary>Filters for Workspace time entries.</summary>
public sealed class GetTimeEntriesRequest
{
    /// <summary>Gets or sets the inclusive start of the range.</summary>
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>Gets or sets the exclusive end of the range.</summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>Gets or sets the assignee user identifier.</summary>
    public long? Assignee { get; set; }

    /// <summary>Gets or sets a value indicating whether task tags are included.</summary>
    public bool? IncludeTaskTags { get; set; }

    /// <summary>Gets or sets a value indicating whether location names are included.</summary>
    public bool? IncludeLocationNames { get; set; }

    /// <summary>Gets or sets a value indicating whether approval history is included.</summary>
    public bool? IncludeApprovalHistory { get; set; }

    /// <summary>Gets or sets a value indicating whether approval details are included.</summary>
    public bool? IncludeApprovalDetails { get; set; }

    /// <summary>Gets or sets a Space filter.</summary>
    public long? SpaceId { get; set; }

    /// <summary>Gets or sets a Folder filter.</summary>
    public long? FolderId { get; set; }

    /// <summary>Gets or sets a List filter.</summary>
    public long? ListId { get; set; }

    /// <summary>Gets or sets a task filter.</summary>
    public string? TaskId { get; set; }

    /// <summary>Gets or sets a value indicating whether the task filter is a custom task identifier.</summary>
    public bool? CustomTaskIds { get; set; }

    /// <summary>Gets or sets the Workspace identifier required with a custom task identifier.</summary>
    public string? TeamId { get; set; }

    /// <summary>Gets or sets a billable filter.</summary>
    public bool? IsBillable { get; set; }
}

/// <summary>Request body for creating a time entry.</summary>
public sealed class CreateTimeEntryRequest
{
    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets tags.</summary>
    [JsonPropertyName("tags")]
    public IReadOnlyList<ClickUpTimeEntryTag>? Tags { get; set; }

    /// <summary>Gets or sets the start time. ClickUp requires this value.</summary>
    [JsonPropertyName("start")]
    public DateTimeOffset? Start { get; set; }

    /// <summary>Gets or sets the stop time.</summary>
    [JsonPropertyName("stop")]
    public DateTimeOffset? Stop { get; set; }

    /// <summary>Gets or sets the end time.</summary>
    [JsonPropertyName("end")]
    public DateTimeOffset? End { get; set; }

    /// <summary>Gets or sets a value indicating whether the time is billable.</summary>
    [JsonPropertyName("billable")]
    public bool? Billable { get; set; }

    /// <summary>Gets or sets the duration in milliseconds. ClickUp requires this value.</summary>
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }

    /// <summary>Gets or sets the assignee user identifier.</summary>
    [JsonPropertyName("assignee")]
    public int? Assignee { get; set; }

    /// <summary>Gets or sets the task identifier.</summary>
    [JsonPropertyName("tid")]
    public string? TaskId { get; set; }
}

/// <summary>Request body for starting a timer.</summary>
public sealed class StartTimeEntryRequest
{
    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets tags.</summary>
    [JsonPropertyName("tags")]
    public IReadOnlyList<ClickUpTimeEntryTag>? Tags { get; set; }

    /// <summary>Gets or sets the task identifier.</summary>
    [JsonPropertyName("tid")]
    public string? TaskId { get; set; }

    /// <summary>Gets or sets a value indicating whether the timer is billable.</summary>
    [JsonPropertyName("billable")]
    public bool? Billable { get; set; }
}

/// <summary>Request body for the legacy task time tracking endpoint.</summary>
public sealed class TrackTimeRequest
{
    /// <summary>Gets or sets the start time.</summary>
    [JsonPropertyName("start")]
    public DateTimeOffset Start { get; set; }

    /// <summary>Gets or sets the end time.</summary>
    [JsonPropertyName("end")]
    public DateTimeOffset End { get; set; }

    /// <summary>Gets or sets the duration in milliseconds.</summary>
    [JsonPropertyName("time")]
    public int Time { get; set; }
}

/// <summary>Tracked time grouped by user for a task.</summary>
public sealed class ClickUpTrackedTime
{
    /// <summary>Gets or sets the user.</summary>
    [JsonPropertyName("user")]
    public ClickUpUser? User { get; set; }

    /// <summary>Gets or sets the total tracked time in milliseconds.</summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>Gets or sets the tracked intervals.</summary>
    [JsonPropertyName("intervals")]
    public IReadOnlyList<ClickUpTimeInterval>? Intervals { get; set; }
}

/// <summary>One interval of tracked time on a task.</summary>
public sealed class ClickUpTimeInterval
{
    /// <summary>Gets or sets the interval identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the start time.</summary>
    [JsonPropertyName("start")]
    public DateTimeOffset? Start { get; set; }

    /// <summary>Gets or sets the end time.</summary>
    [JsonPropertyName("end")]
    public DateTimeOffset? End { get; set; }

    /// <summary>Gets or sets the duration.</summary>
    [JsonPropertyName("time")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? Time { get; set; }

    /// <summary>Gets or sets the source.</summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>Gets or sets the time the interval was added.</summary>
    [JsonPropertyName("date_added")]
    public DateTimeOffset? DateAdded { get; set; }
}

internal sealed class ClickUpTimeEntriesResponse
{
    [JsonPropertyName("data")]
    public IReadOnlyList<ClickUpTimeEntry>? Data { get; set; }
}

internal sealed class ClickUpTimeEntryResponse
{
    [JsonPropertyName("data")]
    public ClickUpTimeEntry? Data { get; set; }
}

internal sealed class ClickUpTrackedTimeResponse
{
    [JsonPropertyName("data")]
    public IReadOnlyList<ClickUpTrackedTime>? Data { get; set; }
}
