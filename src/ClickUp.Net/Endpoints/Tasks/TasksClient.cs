using System.Runtime.CompilerServices;
using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Tasks;

/// <summary>
/// Creates, queries, and updates ClickUp tasks.
/// </summary>
public interface ITasksClient
{
    /// <summary>
    /// Gets one page of tasks in a List.
    /// </summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="request">Optional filters and paging. Page numbers start at 0.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<GetTasksResponse> GetTasksAsync(string listId, GetTasksRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams every task in a List by requesting pages until ClickUp reports the last page.
    /// </summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="request">Optional filters. The page number is controlled by this method.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    IAsyncEnumerable<ClickUpTask> GetAllTasksAsync(string listId, GetTasksRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets one page of tasks filtered across a Workspace.
    /// </summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="request">Optional filters and paging.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<GetTasksResponse> GetFilteredTasksAsync(string teamId, GetFilteredTasksRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a task.
    /// </summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="request">The task to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpTask> CreateTaskAsync(string listId, CreateTaskRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a task by identifier.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="includeSubtasks">When true, subtasks are included.</param>
    /// <param name="includeMarkdownDescription">When true, the Markdown description is included.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpTask> GetTaskAsync(
        string taskId,
        ClickUpTaskReferenceOptions? options = null,
        bool? includeSubtasks = null,
        bool? includeMarkdownDescription = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="request">The fields to update.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpTask> UpdateTaskAsync(
        string taskId,
        UpdateTaskRequest request,
        ClickUpTaskReferenceOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteTaskAsync(string taskId, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);
}

internal sealed class TasksClient : ITasksClient
{
    private const int MaxPages = 1000;
    private readonly IClickUpHttpClient _http;

    public TasksClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<GetTasksResponse> GetTasksAsync(string listId, GetTasksRequest? request = null, CancellationToken cancellationToken = default)
    {
        var endpoint = BuildListQuery(request).Apply("list/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(listId, nameof(listId))) + "/task");
        var response = await _http.GetAsync<GetTasksResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        response = ApiResults.Required(response, "tasks");
        response.Tasks ??= Array.Empty<ClickUpTask>();
        response.Page = request?.Page ?? 0;
        return response;
    }

    public async IAsyncEnumerable<ClickUpTask> GetAllTasksAsync(
        string listId,
        GetTasksRequest? request = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var startPage = request?.Page ?? 0;
        for (var page = startPage; page < startPage + MaxPages; page++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pageRequest = (request ?? new GetTasksRequest()).WithPage(page);
            var response = await GetTasksAsync(listId, pageRequest, cancellationToken).ConfigureAwait(false);
            foreach (var task in response.Tasks)
            {
                yield return task;
            }

            if (!response.HasMore || response.Tasks.Count == 0)
            {
                yield break;
            }
        }

        throw new InvalidOperationException("ClickUp task pagination exceeded 1000 pages.");
    }

    public async Task<GetTasksResponse> GetFilteredTasksAsync(string teamId, GetFilteredTasksRequest? request = null, CancellationToken cancellationToken = default)
    {
        var endpoint = BuildFilteredQuery(request).Apply("team/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(teamId, nameof(teamId))) + "/task");
        var response = await _http.GetAsync<GetTasksResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        response = ApiResults.Required(response, "tasks");
        response.Tasks ??= Array.Empty<ClickUpTask>();
        response.Page = request?.Page ?? 0;
        return response;
    }

    public async Task<ClickUpTask> CreateTaskAsync(string listId, CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(listId, nameof(listId));
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.Name, nameof(request.Name));
        var response = await _http.PostAsync<CreateTaskRequest, ClickUpTask>(
            "list/" + Uri.EscapeDataString(listId) + "/task",
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "task");
    }

    public async Task<ClickUpTask> GetTaskAsync(
        string taskId,
        ClickUpTaskReferenceOptions? options = null,
        bool? includeSubtasks = null,
        bool? includeMarkdownDescription = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ClickUpQuery()
            .Add("include_subtasks", includeSubtasks)
            .Add("include_markdown_description", includeMarkdownDescription);
        ApiResults.ApplyTaskReference(query, options);
        var endpoint = query.Apply("task/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(taskId, nameof(taskId))));
        var response = await _http.GetAsync<ClickUpTask>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "task");
    }

    public async Task<ClickUpTask> UpdateTaskAsync(
        string taskId,
        UpdateTaskRequest request,
        ClickUpTaskReferenceOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(taskId, nameof(taskId));
        Guard.NotNull(request, nameof(request));
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        var response = await _http.PutAsync<UpdateTaskRequest, ClickUpTask>(
            query.Apply("task/" + Uri.EscapeDataString(taskId)),
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "task");
    }

    public Task DeleteTaskAsync(string taskId, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        return _http.DeleteAsync(query.Apply("task/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(taskId, nameof(taskId)))), cancellationToken);
    }

    private static ClickUpQuery BuildListQuery(GetTasksRequest? request)
    {
        var query = new ClickUpQuery();
        if (request is null)
        {
            return query;
        }

        return query
            .Add("archived", request.Archived)
            .Add("include_markdown_description", request.IncludeMarkdownDescription)
            .Add("page", request.Page)
            .Add("order_by", request.OrderBy)
            .Add("reverse", request.Reverse)
            .Add("subtasks", request.Subtasks)
            .AddMany("statuses", request.Statuses)
            .Add("include_closed", request.IncludeClosed)
            .Add("include_timl", request.IncludeTiml)
            .AddMany("assignees", request.Assignees)
            .AddMany("watchers", request.Watchers)
            .AddMany("tags", request.Tags)
            .Add("due_date_gt", request.DueDateGreaterThan)
            .Add("due_date_lt", request.DueDateLessThan)
            .Add("date_created_gt", request.DateCreatedGreaterThan)
            .Add("date_created_lt", request.DateCreatedLessThan)
            .Add("date_updated_gt", request.DateUpdatedGreaterThan)
            .Add("date_updated_lt", request.DateUpdatedLessThan)
            .Add("date_done_gt", request.DateDoneGreaterThan)
            .Add("date_done_lt", request.DateDoneLessThan)
            .Add("custom_fields", ApiResults.SerializeCustomFieldFilters(request.CustomFields))
            .AddMany("custom_items", request.CustomItems);
    }

    private static ClickUpQuery BuildFilteredQuery(GetFilteredTasksRequest? request)
    {
        var query = new ClickUpQuery();
        if (request is null)
        {
            return query;
        }

        return query
            .Add("page", request.Page)
            .Add("order_by", request.OrderBy)
            .Add("reverse", request.Reverse)
            .Add("subtasks", request.Subtasks)
            .AddMany("space_ids[]", request.SpaceIds)
            .AddMany("project_ids[]", request.FolderIds)
            .AddMany("list_ids[]", request.ListIds)
            .AddMany("statuses[]", request.Statuses)
            .Add("include_closed", request.IncludeClosed)
            .AddMany("assignees[]", request.Assignees)
            .AddMany("tags[]", request.Tags)
            .Add("due_date_gt", request.DueDateGreaterThan)
            .Add("due_date_lt", request.DueDateLessThan)
            .Add("date_created_gt", request.DateCreatedGreaterThan)
            .Add("date_created_lt", request.DateCreatedLessThan)
            .Add("date_updated_gt", request.DateUpdatedGreaterThan)
            .Add("date_updated_lt", request.DateUpdatedLessThan)
            .Add("date_done_gt", request.DateDoneGreaterThan)
            .Add("date_done_lt", request.DateDoneLessThan)
            .Add("custom_fields", ApiResults.SerializeCustomFieldFilters(request.CustomFields))
            .Add("parent", request.Parent)
            .Add("include_markdown_description", request.IncludeMarkdownDescription)
            .AddMany("custom_items[]", request.CustomItems);
    }
}
