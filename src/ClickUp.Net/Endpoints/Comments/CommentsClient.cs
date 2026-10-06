using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Comments;

/// <summary>
/// Reads and writes ClickUp comments.
/// </summary>
public interface ICommentsClient
{
    /// <summary>Gets comments on a task.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="request">Optional paging filters.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpComment>> GetTaskCommentsAsync(string taskId, GetCommentsRequest? request = null, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a comment on a task.</summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="request">The comment to create.</param>
    /// <param name="options">Optional custom task id settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpCommentCreated> CreateTaskCommentAsync(string taskId, CreateCommentRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>Gets comments on a List.</summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="request">Optional paging filters.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpComment>> GetListCommentsAsync(string listId, GetCommentsRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a comment on a List.</summary>
    /// <param name="listId">The List identifier.</param>
    /// <param name="request">The comment to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpCommentCreated> CreateListCommentAsync(string listId, CreateCommentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets threaded replies on a comment.</summary>
    /// <param name="commentId">The parent comment identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpComment>> GetThreadedCommentsAsync(string commentId, CancellationToken cancellationToken = default);

    /// <summary>Creates a threaded reply.</summary>
    /// <param name="commentId">The parent comment identifier.</param>
    /// <param name="request">The reply to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpCommentCreated> CreateThreadedCommentAsync(string commentId, CreateCommentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates a comment.</summary>
    /// <param name="commentId">The comment identifier.</param>
    /// <param name="request">The fields to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task UpdateCommentAsync(string commentId, UpdateCommentRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a comment.</summary>
    /// <param name="commentId">The comment identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default);
}

internal sealed class CommentsClient : ICommentsClient
{
    private readonly IClickUpHttpClient _http;

    public CommentsClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ClickUpComment>> GetTaskCommentsAsync(string taskId, GetCommentsRequest? request = null, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = CommentQuery(request);
        ApiResults.ApplyTaskReference(query, options);
        var endpoint = query.Apply("task/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(taskId, nameof(taskId))) + "/comment");
        var response = await _http.GetAsync<ClickUpCommentsResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Comments);
    }

    public Task<ClickUpCommentCreated> CreateTaskCommentAsync(string taskId, CreateCommentRequest request, ClickUpTaskReferenceOptions? options = null, CancellationToken cancellationToken = default)
    {
        var query = new ClickUpQuery();
        ApiResults.ApplyTaskReference(query, options);
        return CreateAsync(query.Apply("task/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(taskId, nameof(taskId))) + "/comment"), request, cancellationToken);
    }

    public async Task<IReadOnlyList<ClickUpComment>> GetListCommentsAsync(string listId, GetCommentsRequest? request = null, CancellationToken cancellationToken = default)
    {
        var endpoint = CommentQuery(request).Apply("list/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(listId, nameof(listId))) + "/comment");
        var response = await _http.GetAsync<ClickUpCommentsResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Comments);
    }

    public Task<ClickUpCommentCreated> CreateListCommentAsync(string listId, CreateCommentRequest request, CancellationToken cancellationToken = default)
    {
        return CreateAsync("list/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(listId, nameof(listId))) + "/comment", request, cancellationToken);
    }

    public async Task<IReadOnlyList<ClickUpComment>> GetThreadedCommentsAsync(string commentId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync<ClickUpCommentsResponse>("comment/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(commentId, nameof(commentId))) + "/reply", cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Comments);
    }

    public Task<ClickUpCommentCreated> CreateThreadedCommentAsync(string commentId, CreateCommentRequest request, CancellationToken cancellationToken = default)
    {
        return CreateAsync("comment/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(commentId, nameof(commentId))) + "/reply", request, cancellationToken);
    }

    public Task UpdateCommentAsync(string commentId, UpdateCommentRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(commentId, nameof(commentId));
        Guard.NotNull(request, nameof(request));
        return _http.PutAsync<UpdateCommentRequest, ClickUpCommentCreated>("comment/" + Uri.EscapeDataString(commentId), request, cancellationToken);
    }

    public Task DeleteCommentAsync(string commentId, CancellationToken cancellationToken = default)
    {
        return _http.DeleteAsync("comment/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(commentId, nameof(commentId))), cancellationToken);
    }

    private async Task<ClickUpCommentCreated> CreateAsync(string endpoint, CreateCommentRequest request, CancellationToken cancellationToken)
    {
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.CommentText, nameof(request.CommentText));
        var response = await _http.PostAsync<CreateCommentRequest, ClickUpCommentCreated>(endpoint, request, cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "comment");
    }

    private static ClickUpQuery CommentQuery(GetCommentsRequest? request)
    {
        var query = new ClickUpQuery();
        if (request is null)
        {
            return query;
        }

        return query.Add("start", request.Start).Add("start_id", request.StartId);
    }
}
