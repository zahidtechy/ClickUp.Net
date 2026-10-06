using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Tags;

/// <summary>
/// Manages Space tags and task tag membership.
/// </summary>
public interface ITagsClient
{
    /// <summary>Gets tags defined in a Space.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpTag>> GetSpaceTagsAsync(string spaceId, CancellationToken cancellationToken = default);

    /// <summary>Creates a Space tag.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="tag">The tag to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task CreateSpaceTagAsync(string spaceId, ClickUpTag tag, CancellationToken cancellationToken = default);

    /// <summary>Updates a Space tag.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="tagName">The current tag name.</param>
    /// <param name="tag">The replacement tag.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task UpdateSpaceTagAsync(string spaceId, string tagName, ClickUpTag tag, CancellationToken cancellationToken = default);

    /// <summary>Deletes a Space tag.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="tagName">The tag name.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteSpaceTagAsync(string spaceId, string tagName, CancellationToken cancellationToken = default);

    /// <summary>Adds a tag to a task.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="tagName">The tag name.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task AddTagToTaskAsync(string taskId, string tagName, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Removes a tag from a task.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="tagName">The tag name.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task RemoveTagFromTaskAsync(string taskId, string tagName, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);
}

internal sealed class TagsClient : ITagsClient
{
    private readonly IClickUpHttpClient _http;

    public TagsClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ClickUpTag>> GetSpaceTagsAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync<ClickUpTagsResponse>("space/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId))) + "/tag", cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Tags);
    }

    public Task CreateSpaceTagAsync(string spaceId, ClickUpTag tag, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId));
        ValidateTag(tag);
        return _http.PostAsync("space/" + Uri.EscapeDataString(spaceId) + "/tag", new ClickUpTagRequest { Tag = tag }, cancellationToken);
    }

    public Task UpdateSpaceTagAsync(string spaceId, string tagName, ClickUpTag tag, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId));
        Guard.NotNullOrWhiteSpace(tagName, nameof(tagName));
        ValidateTag(tag);
        return _http.PutAsync<ClickUpTagRequest, ClickUpTag>(
            "space/" + Uri.EscapeDataString(spaceId) + "/tag/" + Uri.EscapeDataString(tagName),
            new ClickUpTagRequest { Tag = tag },
            cancellationToken);
    }

    public Task DeleteSpaceTagAsync(string spaceId, string tagName, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId));
        Guard.NotNullOrWhiteSpace(tagName, nameof(tagName));
        return _http.DeleteAsync("space/" + Uri.EscapeDataString(spaceId) + "/tag/" + Uri.EscapeDataString(tagName), cancellationToken);
    }

    public Task AddTagToTaskAsync(string taskId, string tagName, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        return _http.PostAsync(
            query.Apply("task/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(taskId, nameof(taskId))) + "/tag/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(tagName, nameof(tagName)))),
            new { },
            cancellationToken);
    }

    public Task RemoveTagFromTaskAsync(string taskId, string tagName, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        return _http.DeleteAsync(
            query.Apply("task/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(taskId, nameof(taskId))) + "/tag/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(tagName, nameof(tagName)))),
            cancellationToken);
    }

    private static void ValidateTag(ClickUpTag tag)
    {
        Guard.NotNull(tag, nameof(tag));
        Guard.NotNullOrWhiteSpace(tag.Name, nameof(tag.Name));
        Guard.NotNullOrWhiteSpace(tag.ForegroundColor, nameof(tag.ForegroundColor));
        Guard.NotNullOrWhiteSpace(tag.BackgroundColor, nameof(tag.BackgroundColor));
    }
}
