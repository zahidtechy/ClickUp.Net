using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Lists;

/// <summary>
/// Creates and manages ClickUp Lists.
/// </summary>
public interface IListsClient
{
    /// <summary>Gets Lists in a Folder.</summary>
    /// <param name="folderId">The Folder identifier.</param>
    /// <param name="archived">When set, filters by archived state.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpList>> GetListsAsync(string folderId, bool? archived = null, CancellationToken cancellationToken = default);

    /// <summary>Gets Lists that live directly in a Space.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="archived">When set, filters by archived state.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpList>> GetFolderlessListsAsync(string spaceId, bool? archived = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a List in a Folder.</summary>
    /// <param name="folderId">The Folder identifier.</param>
    /// <param name="request">The List to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpList> CreateListAsync(string folderId, CreateListRequest request, CancellationToken cancellationToken = default);

    /// <summary>Creates a List directly in a Space.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="request">The List to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpList> CreateFolderlessListAsync(string spaceId, CreateListRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets a List.</summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpList> GetListAsync(string listId, CancellationToken cancellationToken = default);

    /// <summary>Updates a List.</summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="request">The fields to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpList> UpdateListAsync(string listId, UpdateListRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a List.</summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteListAsync(string listId, CancellationToken cancellationToken = default);
}

internal sealed class ListsClient : IListsClient
{
    private readonly IClickUpHttpClient _http;

    public ListsClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ClickUpList>> GetListsAsync(string folderId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        var endpoint = new ClickUpQuery()
            .Add("archived", archived)
            .Apply("folder/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(folderId, nameof(folderId))) + "/list");
        var response = await _http.GetAsync<ClickUpListsResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Lists);
    }

    public async Task<IReadOnlyList<ClickUpList>> GetFolderlessListsAsync(string spaceId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        var endpoint = new ClickUpQuery()
            .Add("archived", archived)
            .Apply("space/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId))) + "/list");
        var response = await _http.GetAsync<ClickUpListsResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Lists);
    }

    public Task<ClickUpList> CreateListAsync(string folderId, CreateListRequest request, CancellationToken cancellationToken = default)
    {
        return CreateAsync("folder/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(folderId, nameof(folderId))) + "/list", request, cancellationToken);
    }

    public Task<ClickUpList> CreateFolderlessListAsync(string spaceId, CreateListRequest request, CancellationToken cancellationToken = default)
    {
        return CreateAsync("space/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId))) + "/list", request, cancellationToken);
    }

    public async Task<ClickUpList> GetListAsync(string listId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync<ClickUpList>("list/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(listId, nameof(listId))), cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "list");
    }

    public async Task<ClickUpList> UpdateListAsync(string listId, UpdateListRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(listId, nameof(listId));
        Guard.NotNull(request, nameof(request));
        var response = await _http.PutAsync<UpdateListRequest, ClickUpList>("list/" + Uri.EscapeDataString(listId), request, cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "list");
    }

    public Task DeleteListAsync(string listId, CancellationToken cancellationToken = default)
    {
        return _http.DeleteAsync("list/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(listId, nameof(listId))), cancellationToken);
    }

    private async Task<ClickUpList> CreateAsync(string endpoint, CreateListRequest request, CancellationToken cancellationToken)
    {
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.Name, nameof(request.Name));
        var response = await _http.PostAsync<CreateListRequest, ClickUpList>(endpoint, request, cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "list");
    }
}
