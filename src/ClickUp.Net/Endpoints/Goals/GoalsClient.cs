using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Goals;

/// <summary>
/// Creates and manages ClickUp Goals.
/// </summary>
public interface IGoalsClient
{
    /// <summary>Gets Goals in a Workspace.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="includeCompleted">When set, filters completed Goals.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<GetGoalsResponse> GetGoalsAsync(string teamId, bool? includeCompleted = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a Goal.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="request">The Goal to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpGoal> CreateGoalAsync(string teamId, CreateGoalRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets a Goal.</summary>
    /// <param name="goalId">The Goal identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpGoal> GetGoalAsync(string goalId, CancellationToken cancellationToken = default);

    /// <summary>Updates a Goal.</summary>
    /// <param name="goalId">The Goal identifier.</param>
    /// <param name="request">The fields to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpGoal> UpdateGoalAsync(string goalId, UpdateGoalRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a Goal.</summary>
    /// <param name="goalId">The Goal identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteGoalAsync(string goalId, CancellationToken cancellationToken = default);
}

internal sealed class GoalsClient : IGoalsClient
{
    private readonly IClickUpHttpClient _http;

    public GoalsClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<GetGoalsResponse> GetGoalsAsync(string teamId, bool? includeCompleted = null, CancellationToken cancellationToken = default)
    {
        var endpoint = new ClickUpQuery()
            .Add("include_completed", includeCompleted)
            .Apply("team/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(teamId, nameof(teamId))) + "/goal");
        var response = await _http.GetAsync<GetGoalsResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        response = ApiResults.Required(response, "goals");
        response.Goals ??= Array.Empty<ClickUpGoal>();
        response.Folders ??= Array.Empty<ClickUpGoalFolder>();
        return response;
    }

    public async Task<ClickUpGoal> CreateGoalAsync(string teamId, CreateGoalRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(teamId, nameof(teamId));
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.Name, nameof(request.Name));
        if (request.DueDate is null)
        {
            throw new ArgumentException("DueDate is required.", nameof(request));
        }

        if (request.Description is null)
        {
            throw new ArgumentException("Description is required.", nameof(request));
        }

        if (request.Owners is null)
        {
            throw new ArgumentException("Owners is required.", nameof(request));
        }

        if (request.Color is null)
        {
            throw new ArgumentException("Color is required.", nameof(request));
        }

        var response = await _http.PostAsync<CreateGoalRequest, ClickUpGoalResponse>(
            "team/" + Uri.EscapeDataString(teamId) + "/goal",
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.Goal, "goal");
    }

    public async Task<ClickUpGoal> GetGoalAsync(string goalId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync<ClickUpGoalResponse>("goal/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(goalId, nameof(goalId))), cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.Goal, "goal");
    }

    public async Task<ClickUpGoal> UpdateGoalAsync(string goalId, UpdateGoalRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(goalId, nameof(goalId));
        Guard.NotNull(request, nameof(request));
        var response = await _http.PutAsync<UpdateGoalRequest, ClickUpGoalResponse>("goal/" + Uri.EscapeDataString(goalId), request, cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.Goal, "goal");
    }

    public Task DeleteGoalAsync(string goalId, CancellationToken cancellationToken = default)
    {
        return _http.DeleteAsync("goal/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(goalId, nameof(goalId))), cancellationToken);
    }
}
