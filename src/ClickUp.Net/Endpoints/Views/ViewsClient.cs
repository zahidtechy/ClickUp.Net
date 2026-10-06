using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Views;

/// <summary>
/// Reads ClickUp views and chat comments on views.
/// </summary>
public interface IViewsClient
{
    /// <summary>Gets views in a Workspace.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpView>> GetTeamViewsAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>Gets views in a Space.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpView>> GetSpaceViewsAsync(string spaceId, CancellationToken cancellationToken = default);

    /// <summary>Gets views in a Folder.</summary>
    /// <param name="folderId">The Folder identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpView>> GetFolderViewsAsync(string folderId, CancellationToken cancellationToken = default);

    /// <summary>Gets views in a List.</summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpView>> GetListViewsAsync(string listId, CancellationToken cancellationToken = default);

    /// <summary>Creates a view in a List.</summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="request">The view to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpView> CreateListViewAsync(string listId, CreateViewRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets a view.</summary>
    /// <param name="viewId">The view identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpView> GetViewAsync(string viewId, CancellationToken cancellationToken = default);

    /// <summary>Deletes a view.</summary>
    /// <param name="viewId">The view identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteViewAsync(string viewId, CancellationToken cancellationToken = default);

    /// <summary>Gets one page of tasks in a view.</summary>
    /// <param name="viewId">The view identifier.</param>
    /// <param name="page">The zero-based page number. ClickUp requires this parameter.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<GetTasksResponse> GetViewTasksAsync(string viewId, int page, CancellationToken cancellationToken = default);

    /// <summary>Gets comments in a chat view.</summary>
    /// <param name="viewId">The view identifier.</param>
    /// <param name="request">Optional paging filters.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpComment>> GetChatViewCommentsAsync(string viewId, GetCommentsRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a comment in a chat view.</summary>
    /// <param name="viewId">The view identifier.</param>
    /// <param name="request">The comment to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpCommentCreated> CreateChatViewCommentAsync(string viewId, CreateCommentRequest request, CancellationToken cancellationToken = default);
}

internal sealed class ViewsClient : IViewsClient
{
    private readonly IClickUpHttpClient _http;

    public ViewsClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public Task<IReadOnlyList<ClickUpView>> GetTeamViewsAsync(string teamId, CancellationToken cancellationToken = default)
    {
        return GetViewsAsync("team/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(teamId, nameof(teamId))) + "/view", cancellationToken);
    }

    public Task<IReadOnlyList<ClickUpView>> GetSpaceViewsAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        return GetViewsAsync("space/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId))) + "/view", cancellationToken);
    }

    public Task<IReadOnlyList<ClickUpView>> GetFolderViewsAsync(string folderId, CancellationToken cancellationToken = default)
    {
        return GetViewsAsync("folder/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(folderId, nameof(folderId))) + "/view", cancellationToken);
    }

    public Task<IReadOnlyList<ClickUpView>> GetListViewsAsync(string listId, CancellationToken cancellationToken = default)
    {
        return GetViewsAsync("list/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(listId, nameof(listId))) + "/view", cancellationToken);
    }

    public async Task<ClickUpView> CreateListViewAsync(string listId, CreateViewRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(listId, nameof(listId));
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.Name, nameof(request.Name));
        Guard.NotNullOrWhiteSpace(request.Type, nameof(request.Type));
        var response = await _http.PostAsync<CreateViewRequest, ClickUpViewResponse>(
            "list/" + Uri.EscapeDataString(listId) + "/view",
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.View, "view");
    }

    public async Task<ClickUpView> GetViewAsync(string viewId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync<ClickUpViewResponse>("view/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(viewId, nameof(viewId))), cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.View, "view");
    }

    public Task DeleteViewAsync(string viewId, CancellationToken cancellationToken = default)
    {
        return _http.DeleteAsync("view/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(viewId, nameof(viewId))), cancellationToken);
    }

    public async Task<GetTasksResponse> GetViewTasksAsync(string viewId, int page, CancellationToken cancellationToken = default)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be zero or greater.");
        }

        var endpoint = new ClickUpQuery()
            .Add("page", page)
            .Apply("view/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(viewId, nameof(viewId))) + "/task");
        var response = await _http.GetAsync<GetTasksResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        response = ApiResults.Required(response, "tasks");
        response.Tasks ??= Array.Empty<ClickUpTask>();
        response.Page = page;
        return response;
    }

    public async Task<IReadOnlyList<ClickUpComment>> GetChatViewCommentsAsync(string viewId, GetCommentsRequest? request = null, CancellationToken cancellationToken = default)
    {
        var query = new ClickUpQuery();
        if (request is not null)
        {
            query.Add("start", request.Start).Add("start_id", request.StartId);
        }

        var endpoint = query.Apply("view/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(viewId, nameof(viewId))) + "/comment");
        var response = await _http.GetAsync<ClickUpCommentsResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Comments);
    }

    public async Task<ClickUpCommentCreated> CreateChatViewCommentAsync(string viewId, CreateCommentRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(viewId, nameof(viewId));
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.CommentText, nameof(request.CommentText));
        var response = await _http.PostAsync<CreateCommentRequest, ClickUpCommentCreated>(
            "view/" + Uri.EscapeDataString(viewId) + "/comment",
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "comment");
    }

    private async Task<IReadOnlyList<ClickUpView>> GetViewsAsync(string endpoint, CancellationToken cancellationToken)
    {
        var response = await _http.GetAsync<ClickUpViewsResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Views);
    }
}
