using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.CustomFields;

/// <summary>
/// Reads custom field definitions and writes task custom field values.
/// </summary>
public interface ICustomFieldsClient
{
    /// <summary>Gets custom fields available on a List.</summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="includeAppliedObjects">When true, each field includes the objects it applies to.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpCustomField>> GetListFieldsAsync(string listId, bool? includeAppliedObjects = null, CancellationToken cancellationToken = default);

    /// <summary>Sets a custom field value on a task.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="fieldId">The custom field identifier.</param>
    /// <param name="request">The value to store.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task SetTaskFieldValueAsync(string taskId, string fieldId, SetCustomFieldValueRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Removes a custom field value from a task.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="fieldId">The custom field identifier.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task RemoveTaskFieldValueAsync(string taskId, string fieldId, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);
}

internal sealed class CustomFieldsClient : ICustomFieldsClient
{
    private readonly IClickUpHttpClient _http;

    public CustomFieldsClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ClickUpCustomField>> GetListFieldsAsync(string listId, bool? includeAppliedObjects = null, CancellationToken cancellationToken = default)
    {
        var endpoint = new ClickUpQuery()
            .Add("include_applied_objects", includeAppliedObjects)
            .Apply("list/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(listId, nameof(listId))) + "/field");
        var response = await _http.GetAsync<ClickUpCustomFieldsResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Fields);
    }

    public Task SetTaskFieldValueAsync(string taskId, string fieldId, SetCustomFieldValueRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(taskId, nameof(taskId));
        Guard.NotNullOrWhiteSpace(fieldId, nameof(fieldId));
        Guard.NotNull(request, nameof(request));
        Guard.NotNull(request.Value, nameof(request.Value));
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        return _http.PostAsync(
            query.Apply("task/" + Uri.EscapeDataString(taskId) + "/field/" + Uri.EscapeDataString(fieldId)),
            request,
            cancellationToken);
    }

    public Task RemoveTaskFieldValueAsync(string taskId, string fieldId, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(taskId, nameof(taskId));
        Guard.NotNullOrWhiteSpace(fieldId, nameof(fieldId));
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        return _http.DeleteAsync(
            query.Apply("task/" + Uri.EscapeDataString(taskId) + "/field/" + Uri.EscapeDataString(fieldId)),
            cancellationToken);
    }
}
