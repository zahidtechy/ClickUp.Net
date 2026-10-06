using ClickUp.Net.Authentication;

namespace ClickUp.Net.Authentication.PersonalToken;

internal sealed class PersonalTokenAuthenticator : IClickUpAuthenticator
{
    private readonly string? _token;

    public PersonalTokenAuthenticator(string? token)
    {
        _token = token;
    }

    public string CreateAuthorizationHeader()
    {
        if (string.IsNullOrWhiteSpace(_token))
        {
            throw new InvalidOperationException("ClickUp personal token is not configured.");
        }

        return _token;
    }
}
