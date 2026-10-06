using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Users;

/// <summary>
/// Reads the authenticated ClickUp user.
/// </summary>
public interface IUsersClient
{
    /// <summary>
    /// Gets the user associated with the current token.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The authorized user.</returns>
    Task<ClickUpUser> GetAuthorizedUserAsync(CancellationToken cancellationToken = default);
}

internal sealed class UsersClient : IUsersClient
{
    private readonly IClickUpHttpClient _http;

    public UsersClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<ClickUpUser> GetAuthorizedUserAsync(CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync<ClickUpUserResponse>("user", cancellationToken).ConfigureAwait(false);
        return ApiResults.Required(response?.User, "authorized user");
    }
}
