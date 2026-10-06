using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// Known <c>order_by</c> values for task list queries.
/// </summary>
public static class ClickUpTaskOrderBy
{
    /// <summary>Order by task identifier.</summary>
    public const string Id = "id";

    /// <summary>Order by creation time.</summary>
    public const string Created = "created";

    /// <summary>Order by update time.</summary>
    public const string Updated = "updated";

    /// <summary>Order by due date.</summary>
    public const string DueDate = "due_date";
}

/// <summary>
/// Filters and paging options for tasks in a List.
/// ClickUp returns at most 100 tasks per page. Page numbers start at 0.
/// </summary>
public sealed class GetTasksRequest
{
    /// <summary>The number of tasks ClickUp returns on one page.</summary>
    public const int PageSize = 100;

    /// <summary>Gets or sets a value indicating whether archived tasks are included.</summary>
    public bool? Archived { get; set; }

    /// <summary>Gets or sets a value indicating whether Markdown descriptions are included.</summary>
    public bool? IncludeMarkdownDescription { get; set; }

    /// <summary>Gets or sets the zero-based page number.</summary>
    public int? Page { get; set; }

    /// <summary>Gets or sets the sort field. See <see cref="ClickUpTaskOrderBy"/>.</summary>
    public string? OrderBy { get; set; }

    /// <summary>Gets or sets a value indicating whether the sort is reversed.</summary>
    public bool? Reverse { get; set; }

    /// <summary>Gets or sets a value indicating whether subtasks are included.</summary>
    public bool? Subtasks { get; set; }

    /// <summary>Gets or sets status names to include.</summary>
    public IReadOnlyList<string>? Statuses { get; set; }

    /// <summary>Gets or sets a value indicating whether closed tasks are included.</summary>
    public bool? IncludeClosed { get; set; }

    /// <summary>Gets or sets a value indicating whether tasks in multiple lists are included.</summary>
    public bool? IncludeTiml { get; set; }

    /// <summary>Gets or sets assignee user identifiers.</summary>
    public IReadOnlyList<string>? Assignees { get; set; }

    /// <summary>Gets or sets watcher user identifiers.</summary>
    public IReadOnlyList<string>? Watchers { get; set; }

    /// <summary>Gets or sets tag names.</summary>
    public IReadOnlyList<string>? Tags { get; set; }

    /// <summary>Gets or sets the exclusive lower bound for due date.</summary>
    public DateTimeOffset? DueDateGreaterThan { get; set; }

    /// <summary>Gets or sets the exclusive upper bound for due date.</summary>
    public DateTimeOffset? DueDateLessThan { get; set; }

    /// <summary>Gets or sets the exclusive lower bound for creation time.</summary>
    public DateTimeOffset? DateCreatedGreaterThan { get; set; }

    /// <summary>Gets or sets the exclusive upper bound for creation time.</summary>
    public DateTimeOffset? DateCreatedLessThan { get; set; }

    /// <summary>Gets or sets the exclusive lower bound for update time.</summary>
    public DateTimeOffset? DateUpdatedGreaterThan { get; set; }

    /// <summary>Gets or sets the exclusive upper bound for update time.</summary>
    public DateTimeOffset? DateUpdatedLessThan { get; set; }

    /// <summary>Gets or sets the exclusive lower bound for done time.</summary>
    public DateTimeOffset? DateDoneGreaterThan { get; set; }

    /// <summary>Gets or sets the exclusive upper bound for done time.</summary>
    public DateTimeOffset? DateDoneLessThan { get; set; }

    /// <summary>Gets or sets custom field filters. They are sent as one JSON query value.</summary>
    public IReadOnlyList<ClickUpCustomFieldFilter>? CustomFields { get; set; }

    /// <summary>Gets or sets custom task type identifiers.</summary>
    public IReadOnlyList<int>? CustomItems { get; set; }

    /// <summary>Creates a copy of this request with a different page number.</summary>
    /// <param name="page">The zero-based page number.</param>
    /// <returns>A copy of the request.</returns>
    public GetTasksRequest WithPage(int page)
    {
        return new GetTasksRequest
        {
            Archived = Archived,
            IncludeMarkdownDescription = IncludeMarkdownDescription,
            Page = page,
            OrderBy = OrderBy,
            Reverse = Reverse,
            Subtasks = Subtasks,
            Statuses = Statuses,
            IncludeClosed = IncludeClosed,
            IncludeTiml = IncludeTiml,
            Assignees = Assignees,
            Watchers = Watchers,
            Tags = Tags,
            DueDateGreaterThan = DueDateGreaterThan,
            DueDateLessThan = DueDateLessThan,
            DateCreatedGreaterThan = DateCreatedGreaterThan,
            DateCreatedLessThan = DateCreatedLessThan,
            DateUpdatedGreaterThan = DateUpdatedGreaterThan,
            DateUpdatedLessThan = DateUpdatedLessThan,
            DateDoneGreaterThan = DateDoneGreaterThan,
            DateDoneLessThan = DateDoneLessThan,
            CustomFields = CustomFields,
            CustomItems = CustomItems
        };
    }
}

/// <summary>
/// Filters for the Workspace task endpoint. Array parameters use the <c>[]</c> names defined by that route.
/// </summary>
public sealed class GetFilteredTasksRequest
{
    /// <summary>Gets or sets the zero-based page number.</summary>
    public int? Page { get; set; }

    /// <summary>Gets or sets the sort field.</summary>
    public string? OrderBy { get; set; }

    /// <summary>Gets or sets a value indicating whether the sort is reversed.</summary>
    public bool? Reverse { get; set; }

    /// <summary>Gets or sets a value indicating whether subtasks are included.</summary>
    public bool? Subtasks { get; set; }

    /// <summary>Gets or sets Space identifiers.</summary>
    public IReadOnlyList<string>? SpaceIds { get; set; }

    /// <summary>Gets or sets Folder identifiers. The API parameter name is <c>project_ids[]</c>.</summary>
    public IReadOnlyList<string>? FolderIds { get; set; }

    /// <summary>Gets or sets List identifiers.</summary>
    public IReadOnlyList<string>? ListIds { get; set; }

    /// <summary>Gets or sets status names.</summary>
    public IReadOnlyList<string>? Statuses { get; set; }

    /// <summary>Gets or sets a value indicating whether closed tasks are included.</summary>
    public bool? IncludeClosed { get; set; }

    /// <summary>Gets or sets assignee identifiers.</summary>
    public IReadOnlyList<string>? Assignees { get; set; }

    /// <summary>Gets or sets tag names.</summary>
    public IReadOnlyList<string>? Tags { get; set; }

    /// <summary>Gets or sets the exclusive lower bound for due date.</summary>
    public DateTimeOffset? DueDateGreaterThan { get; set; }

    /// <summary>Gets or sets the exclusive upper bound for due date.</summary>
    public DateTimeOffset? DueDateLessThan { get; set; }

    /// <summary>Gets or sets the exclusive lower bound for creation time.</summary>
    public DateTimeOffset? DateCreatedGreaterThan { get; set; }

    /// <summary>Gets or sets the exclusive upper bound for creation time.</summary>
    public DateTimeOffset? DateCreatedLessThan { get; set; }

    /// <summary>Gets or sets the exclusive lower bound for update time.</summary>
    public DateTimeOffset? DateUpdatedGreaterThan { get; set; }

    /// <summary>Gets or sets the exclusive upper bound for update time.</summary>
    public DateTimeOffset? DateUpdatedLessThan { get; set; }

    /// <summary>Gets or sets the exclusive lower bound for done time.</summary>
    public DateTimeOffset? DateDoneGreaterThan { get; set; }

    /// <summary>Gets or sets the exclusive upper bound for done time.</summary>
    public DateTimeOffset? DateDoneLessThan { get; set; }

    /// <summary>Gets or sets custom field filters.</summary>
    public IReadOnlyList<ClickUpCustomFieldFilter>? CustomFields { get; set; }

    /// <summary>Gets or sets a parent task identifier.</summary>
    public string? Parent { get; set; }

    /// <summary>Gets or sets a value indicating whether Markdown descriptions are included.</summary>
    public bool? IncludeMarkdownDescription { get; set; }

    /// <summary>Gets or sets custom task type identifiers.</summary>
    public IReadOnlyList<int>? CustomItems { get; set; }

    /// <summary>Creates a copy of this request with a different page number.</summary>
    /// <param name="page">The zero-based page number.</param>
    /// <returns>A copy of the request.</returns>
    public GetFilteredTasksRequest WithPage(int page)
    {
        return new GetFilteredTasksRequest
        {
            Page = page,
            OrderBy = OrderBy,
            Reverse = Reverse,
            Subtasks = Subtasks,
            SpaceIds = SpaceIds,
            FolderIds = FolderIds,
            ListIds = ListIds,
            Statuses = Statuses,
            IncludeClosed = IncludeClosed,
            Assignees = Assignees,
            Tags = Tags,
            DueDateGreaterThan = DueDateGreaterThan,
            DueDateLessThan = DueDateLessThan,
            DateCreatedGreaterThan = DateCreatedGreaterThan,
            DateCreatedLessThan = DateCreatedLessThan,
            DateUpdatedGreaterThan = DateUpdatedGreaterThan,
            DateUpdatedLessThan = DateUpdatedLessThan,
            DateDoneGreaterThan = DateDoneGreaterThan,
            DateDoneLessThan = DateDoneLessThan,
            CustomFields = CustomFields,
            Parent = Parent,
            IncludeMarkdownDescription = IncludeMarkdownDescription,
            CustomItems = CustomItems
        };
    }
}

/// <summary>
/// Options for addressing a task by its custom identifier.
/// </summary>
public sealed class ClickUpTaskReferenceOptions
{
    /// <summary>Gets or sets a value indicating whether <c>taskId</c> is a custom task identifier.</summary>
    public bool? CustomTaskIds { get; set; }

    /// <summary>Gets or sets the Workspace identifier. ClickUp requires this when <see cref="CustomTaskIds"/> is true.</summary>
    public string? TeamId { get; set; }
}

/// <summary>
/// A page of tasks. ClickUp uses <c>last_page</c> rather than a total count.
/// </summary>
public sealed class GetTasksResponse
{
    /// <summary>Gets or sets the tasks on this page.</summary>
    [JsonPropertyName("tasks")]
    public IReadOnlyList<ClickUpTask> Tasks { get; set; } = Array.Empty<ClickUpTask>();

    /// <summary>Gets or sets a value indicating whether this is the last page, when ClickUp returned the flag.</summary>
    [JsonPropertyName("last_page")]
    public bool? LastPage { get; set; }

    /// <summary>Gets or sets the page number that was requested.</summary>
    [JsonIgnore]
    public int Page { get; set; }

    /// <summary>
    /// Gets a value indicating whether another page should be requested.
    /// When <see cref="LastPage"/> is missing, another page is assumed while this page is full.
    /// </summary>
    [JsonIgnore]
    public bool HasMore => LastPage.HasValue ? !LastPage.Value : Tasks.Count >= GetTasksRequest.PageSize;
}
