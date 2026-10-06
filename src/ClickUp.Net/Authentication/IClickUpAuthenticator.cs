namespace ClickUp.Net.Authentication;

internal interface IClickUpAuthenticator
{
    string CreateAuthorizationHeader();
}
