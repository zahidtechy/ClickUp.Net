using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Teams;

/// <summary>
/// Reads ClickUp teams, also called Workspaces.
/// </summary>
public interface ITeamsClient
{
    /// <summary>
    /// Gets the Workspaces available to the current token.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The authorized Workspaces.</returns>
    Task<IReadOnlyList<ClickUpTeam>> GetTeamsAsync(CancellationToken cancellationToken = default);
}

internal sealed class TeamsClient : ITeamsClient
{
    private readonly IClickUpHttpClient _http;

    public TeamsClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ClickUpTeam>> GetTeamsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync<ClickUpTeamsResponse>("team", cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Teams);
    }
}
