using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Checklists;

/// <summary>
/// Creates and updates task checklists.
/// </summary>
public interface IChecklistsClient
{
    /// <summary>Creates a checklist on a task.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="request">The checklist to create.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpChecklist> CreateChecklistAsync(string taskId, CreateChecklistRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Updates a checklist. ClickUp returns an empty JSON object for this route.</summary>
    /// <param name="checklistId">The checklist identifier.</param>
    /// <param name="request">The fields to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task UpdateChecklistAsync(string checklistId, UpdateChecklistRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a checklist.</summary>
    /// <param name="checklistId">The checklist identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteChecklistAsync(string checklistId, CancellationToken cancellationToken = default);

    /// <summary>Creates a checklist item.</summary>
    /// <param name="checklistId">The checklist identifier.</param>
    /// <param name="request">The item to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpChecklist> CreateChecklistItemAsync(string checklistId, CreateChecklistItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates a checklist item.</summary>
    /// <param name="checklistId">The checklist identifier.</param>
    /// <param name="checklistItemId">The checklist item identifier.</param>
    /// <param name="request">The fields to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpChecklist> UpdateChecklistItemAsync(string checklistId, string checklistItemId, UpdateChecklistItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a checklist item.</summary>
    /// <param name="checklistId">The checklist identifier.</param>
    /// <param name="checklistItemId">The checklist item identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteChecklistItemAsync(string checklistId, string checklistItemId, CancellationToken cancellationToken = default);
}

internal sealed class ChecklistsClient : IChecklistsClient
{
    private readonly IClickUpHttpClient _http;

    public ChecklistsClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<ClickUpChecklist> CreateChecklistAsync(string taskId, CreateChecklistRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.Name, nameof(request.Name));
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        var response = await _http.PostAsync<CreateChecklistRequest, ClickUpChecklistResponse>(
            query.Apply("task/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(taskId, nameof(taskId))) + "/checklist"),
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.Checklist, "checklist");
    }

    public Task UpdateChecklistAsync(string checklistId, UpdateChecklistRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(checklistId, nameof(checklistId));
        Guard.NotNull(request, nameof(request));
        return _http.PutAsync<UpdateChecklistRequest, System.Text.Json.JsonElement>("checklist/" + Uri.EscapeDataString(checklistId), request, cancellationToken);
    }

    public Task DeleteChecklistAsync(string checklistId, CancellationToken cancellationToken = default)
    {
        return _http.DeleteAsync("checklist/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(checklistId, nameof(checklistId))), cancellationToken);
    }

    public async Task<ClickUpChecklist> CreateChecklistItemAsync(string checklistId, CreateChecklistItemRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(checklistId, nameof(checklistId));
        Guard.NotNull(request, nameof(request));
        var response = await _http.PostAsync<CreateChecklistItemRequest, ClickUpChecklistResponse>(
            "checklist/" + Uri.EscapeDataString(checklistId) + "/checklist_item",
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.Checklist, "checklist");
    }

    public async Task<ClickUpChecklist> UpdateChecklistItemAsync(string checklistId, string checklistItemId, UpdateChecklistItemRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(checklistId, nameof(checklistId));
        Guard.NotNullOrWhiteSpace(checklistItemId, nameof(checklistItemId));
        Guard.NotNull(request, nameof(request));
        var response = await _http.PutAsync<UpdateChecklistItemRequest, ClickUpChecklistResponse>(
            "checklist/" + Uri.EscapeDataString(checklistId) + "/checklist_item/" + Uri.EscapeDataString(checklistItemId),
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.Checklist, "checklist");
    }

    public Task DeleteChecklistItemAsync(string checklistId, string checklistItemId, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(checklistId, nameof(checklistId));
        Guard.NotNullOrWhiteSpace(checklistItemId, nameof(checklistItemId));
        return _http.DeleteAsync(
            "checklist/" + Uri.EscapeDataString(checklistId) + "/checklist_item/" + Uri.EscapeDataString(checklistItemId),
            cancellationToken);
    }
}
