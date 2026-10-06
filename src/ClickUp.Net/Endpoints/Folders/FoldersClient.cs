using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Folders;

/// <summary>
/// Creates and manages ClickUp Folders.
/// </summary>
public interface IFoldersClient
{
    /// <summary>Gets Folders in a Space.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="archived">When set, filters by archived state.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpFolder>> GetFoldersAsync(string spaceId, bool? archived = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a Folder.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="request">The Folder to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpFolder> CreateFolderAsync(string spaceId, CreateFolderRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets a Folder.</summary>
    /// <param name="folderId">The Folder identifier.</param>
    /// <param name="includeSubfolders">When true, nested Folders are included.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpFolder> GetFolderAsync(string folderId, bool? includeSubfolders = null, CancellationToken cancellationToken = default);

    /// <summary>Updates a Folder.</summary>
    /// <param name="folderId">The Folder identifier.</param>
    /// <param name="request">The fields to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpFolder> UpdateFolderAsync(string folderId, UpdateFolderRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a Folder.</summary>
    /// <param name="folderId">The Folder identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteFolderAsync(string folderId, CancellationToken cancellationToken = default);
}

internal sealed class FoldersClient : IFoldersClient
{
    private readonly IClickUpHttpClient _http;

    public FoldersClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ClickUpFolder>> GetFoldersAsync(string spaceId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        var endpoint = new ClickUpQuery()
            .Add("archived", archived)
            .Apply("space/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId))) + "/folder");
        var response = await _http.GetAsync<ClickUpFoldersResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Folders);
    }

    public async Task<ClickUpFolder> CreateFolderAsync(string spaceId, CreateFolderRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId));
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.Name, nameof(request.Name));
        var response = await _http.PostAsync<CreateFolderRequest, ClickUpFolder>(
            "space/" + Uri.EscapeDataString(spaceId) + "/folder",
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "folder");
    }

    public async Task<ClickUpFolder> GetFolderAsync(string folderId, bool? includeSubfolders = null, CancellationToken cancellationToken = default)
    {
        var endpoint = new ClickUpQuery()
            .Add("include_subfolders", includeSubfolders)
            .Apply("folder/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(folderId, nameof(folderId))));
        var response = await _http.GetAsync<ClickUpFolder>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "folder");
    }

    public async Task<ClickUpFolder> UpdateFolderAsync(string folderId, UpdateFolderRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(folderId, nameof(folderId));
        Guard.NotNull(request, nameof(request));
        var response = await _http.PutAsync<UpdateFolderRequest, ClickUpFolder>("folder/" + Uri.EscapeDataString(folderId), request, cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "folder");
    }

    public Task DeleteFolderAsync(string folderId, CancellationToken cancellationToken = default)
    {
        return _http.DeleteAsync("folder/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(folderId, nameof(folderId))), cancellationToken);
    }
}
