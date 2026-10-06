using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.TimeTracking;

/// <summary>
/// Reads and writes ClickUp time entries.
/// </summary>
public interface ITimeTrackingClient
{
    /// <summary>Gets time entries in a Workspace.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="request">Optional filters.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpTimeEntry>> GetTimeEntriesAsync(string teamId, GetTimeEntriesRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a time entry.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="request">The time entry to create.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<CreateTimeEntryRequest> CreateTimeEntryAsync(string teamId, CreateTimeEntryRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Gets the running timer, or null when no timer is running.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="assignee">Optional assignee user identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpTimeEntry?> GetRunningTimeEntryAsync(string teamId, long? assignee = null, CancellationToken cancellationToken = default);

    /// <summary>Starts a timer.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="request">The timer to start.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpTimeEntry> StartTimeEntryAsync(string teamId, StartTimeEntryRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Stops the running timer.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpTimeEntry> StopTimeEntryAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>Gets tracked time for a task.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpTrackedTime>> GetTrackedTimeAsync(string taskId, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Adds tracked time to a task through the task time endpoint.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="request">The interval to add.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task TrackTimeAsync(string taskId, TrackTimeRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Deletes a tracked time interval from a task.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="intervalId">The interval identifier.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteTrackedTimeAsync(string taskId, string intervalId, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);
}

internal sealed class TimeTrackingClient : ITimeTrackingClient
{
    private readonly IClickUpHttpClient _http;

    public TimeTrackingClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ClickUpTimeEntry>> GetTimeEntriesAsync(string teamId, GetTimeEntriesRequest? request = null, CancellationToken cancellationToken = default)
    {
        var query = new ClickUpQuery();
        if (request is not null)
        {
            if (request.CustomTaskIds == true && string.IsNullOrWhiteSpace(request.TeamId))
            {
                throw new ArgumentException("TeamId is required when CustomTaskIds is true.", nameof(request));
            }

            query.Add("start_date", request.StartDate)
                .Add("end_date", request.EndDate)
                .Add("assignee", request.Assignee)
                .Add("include_task_tags", request.IncludeTaskTags)
                .Add("include_location_names", request.IncludeLocationNames)
                .Add("include_approval_history", request.IncludeApprovalHistory)
                .Add("include_approval_details", request.IncludeApprovalDetails)
                .Add("space_id", request.SpaceId)
                .Add("folder_id", request.FolderId)
                .Add("list_id", request.ListId)
                .Add("task_id", request.TaskId)
                .Add("custom_task_ids", request.CustomTaskIds)
                .Add("team_id", request.TeamId)
                .Add("is_billable", request.IsBillable);
        }

        var endpoint = query.Apply("team/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(teamId, nameof(teamId))) + "/time_entries");
        var response = await _http.GetAsync<ClickUpTimeEntriesResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Data);
    }

    public async Task<CreateTimeEntryRequest> CreateTimeEntryAsync(string teamId, CreateTimeEntryRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(teamId, nameof(teamId));
        Guard.NotNull(request, nameof(request));
        if (request.Start is null)
        {
            throw new ArgumentException("Start is required.", nameof(request));
        }

        if (request.Duration is null)
        {
            throw new ArgumentException("Duration is required.", nameof(request));
        }

        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        var response = await _http.PostAsync<CreateTimeEntryRequest, CreateTimeEntryRequest>(
            query.Apply("team/" + Uri.EscapeDataString(teamId) + "/time_entries"),
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "time entry");
    }

    public async Task<ClickUpTimeEntry?> GetRunningTimeEntryAsync(string teamId, long? assignee = null, CancellationToken cancellationToken = default)
    {
        var endpoint = new ClickUpQuery()
            .Add("assignee", assignee)
            .Apply("team/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(teamId, nameof(teamId))) + "/time_entries/current");
        var response = await _http.GetAsync<ClickUpTimeEntryResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return response?.Data;
    }

    public async Task<ClickUpTimeEntry> StartTimeEntryAsync(string teamId, StartTimeEntryRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(teamId, nameof(teamId));
        Guard.NotNull(request, nameof(request));
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        var response = await _http.PostAsync<StartTimeEntryRequest, ClickUpTimeEntryResponse>(
            query.Apply("team/" + Uri.EscapeDataString(teamId) + "/time_entries/start"),
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.Data, "time entry");
    }

    public async Task<ClickUpTimeEntry> StopTimeEntryAsync(string teamId, CancellationToken cancellationToken = default)
    {
        var response = await _http.PostAsync<object, ClickUpTimeEntryResponse>(
            "team/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(teamId, nameof(teamId))) + "/time_entries/stop",
            new { },
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.Data, "time entry");
    }

    public async Task<IReadOnlyList<ClickUpTrackedTime>> GetTrackedTimeAsync(string taskId, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        var response = await _http.GetAsync<ClickUpTrackedTimeResponse>(
            query.Apply("task/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(taskId, nameof(taskId))) + "/time"),
            cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Data);
    }

    public Task TrackTimeAsync(string taskId, TrackTimeRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(taskId, nameof(taskId));
        Guard.NotNull(request, nameof(request));
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        return _http.PostAsync(query.Apply("task/" + Uri.EscapeDataString(taskId) + "/time"), request, cancellationToken);
    }

    public Task DeleteTrackedTimeAsync(string taskId, string intervalId, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(taskId, nameof(taskId));
        Guard.NotNullOrWhiteSpace(intervalId, nameof(intervalId));
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        return _http.DeleteAsync(
            query.Apply("task/" + Uri.EscapeDataString(taskId) + "/time/" + Uri.EscapeDataString(intervalId)),
            cancellationToken);
    }
}
