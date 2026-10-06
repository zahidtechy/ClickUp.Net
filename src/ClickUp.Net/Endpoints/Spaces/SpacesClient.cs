using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Spaces;

/// <summary>
/// Creates and manages ClickUp Spaces.
/// </summary>
public interface ISpacesClient
{
    /// <summary>Gets Spaces in a Workspace.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="archived">When set, filters by archived state.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpSpace>> GetSpacesAsync(string teamId, bool? archived = null, CancellationToken cancellationToken = default);

    /// <summary>Creates a Space.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="request">The Space to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpSpace> CreateSpaceAsync(string teamId, CreateSpaceRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets a Space by identifier.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpSpace> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default);

    /// <summary>Updates a Space.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="request">The fields to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpSpace> UpdateSpaceAsync(string spaceId, UpdateSpaceRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a Space.</summary>
    /// <param name="spaceId">The Space identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteSpaceAsync(string spaceId, CancellationToken cancellationToken = default);
}

internal sealed class SpacesClient : ISpacesClient
{
    private readonly IClickUpHttpClient _http;

    public SpacesClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ClickUpSpace>> GetSpacesAsync(string teamId, bool? archived = null, CancellationToken cancellationToken = default)
    {
        var endpoint = new ClickUpQuery()
            .Add("archived", archived)
            .Apply("team/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(teamId, nameof(teamId))) + "/space");
        var response = await _http.GetAsync<ClickUpSpacesResponse>(endpoint, cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Spaces);
    }

    public async Task<ClickUpSpace> CreateSpaceAsync(string teamId, CreateSpaceRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(teamId, nameof(teamId));
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.Name, nameof(request.Name));
        if (request.Features is null)
        {
            throw new ArgumentException("ClickUp requires a features object when creating a Space.", nameof(request));
        }

        var response = await _http.PostAsync<CreateSpaceRequest, ClickUpSpace>(
            "team/" + Uri.EscapeDataString(teamId) + "/space",
            request,
            cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "space");
    }

    public async Task<ClickUpSpace> GetSpaceAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync<ClickUpSpace>("space/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId))), cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "space");
    }

    public async Task<ClickUpSpace> UpdateSpaceAsync(string spaceId, UpdateSpaceRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId));
        Guard.NotNull(request, nameof(request));
        var response = await _http.PutAsync<UpdateSpaceRequest, ClickUpSpace>("space/" + Uri.EscapeDataString(spaceId), request, cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response, "space");
    }

    public Task DeleteSpaceAsync(string spaceId, CancellationToken cancellationToken = default)
    {
        return _http.DeleteAsync("space/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(spaceId, nameof(spaceId))), cancellationToken);
    }
}
