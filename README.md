# ClickUp.Net

A strongly typed .NET 6 client for the [ClickUp API v2](https://developer.clickup.com/reference).

The library sends authentication, builds request URLs, serializes JSON, and surfaces ClickUp error payloads. Application code works with endpoint clients and models instead of `HttpRequestMessage`.

## Installation

```bash
dotnet add package ClickUp.Net
```

The package is licensed under the MIT License.

## Personal-token setup

Personal tokens are sent in the `Authorization` header without a `Bearer` prefix, which is what the ClickUp API expects.

```csharp
builder.Services.AddClickUp(options =>
{
    options.PersonalToken = builder.Configuration["ClickUp:PersonalToken"];
});
```

Configuration binding:

```csharp
builder.Services.AddClickUp(builder.Configuration.GetSection("ClickUp"));
```

```json
{
  "ClickUp": {
    "PersonalToken": "pk_xxxxx",
    "BaseUrl": "https://api.clickup.com/api/v2/"
  }
}
```

`AddClickUp` registers a named `HttpClient` through `IHttpClientFactory`. The client is safe to reuse. Do not create a new `HttpClient` per request.

## Dependency injection

```csharp
public class TaskService
{
    private readonly IClickUpClient _clickUp;

    public TaskService(IClickUpClient clickUp)
    {
        _clickUp = clickUp;
    }

    public async Task GetTaskAsync(string taskId)
    {
        var task = await _clickUp.Tasks.GetTaskAsync(taskId);
    }
}
```

## Creating a task

```csharp
var request = new CreateTaskRequest
{
    Name = "Test task",
    Description = "Created from ClickUp.Net"
};

var task = await clickUp.Tasks.CreateTaskAsync(listId, request);
```

Null properties are omitted. Update requests use `Optional<T>` so an omitted property is not sent, while an explicit null is sent as JSON null.

Dates are `DateTimeOffset` values in the SDK and are written as Unix timestamps in milliseconds.

## OAuth

1. Send the user to the authorization URL.
2. Read the `code` query parameter on your redirect URI.
3. Exchange that code for an access token.
4. Create a client with the access token.

```csharp
var oauth = serviceProvider.GetRequiredService<IClickUpOAuthClient>();

var authorizationUrl = oauth.GetAuthorizationUrl(clientId, redirectUri);

var token = await oauth.ExchangeCodeForTokenAsync(
    clientId,
    clientSecret,
    authorizationCode);

var factory = serviceProvider.GetRequiredService<IClickUpClientFactory>();
var clickUp = factory.CreateWithAccessToken(token.AccessToken);

var user = await clickUp.Users.GetAuthorizedUserAsync();
```

You can also configure one shared access token:

```csharp
builder.Services.AddClickUp(options =>
{
    options.AuthenticationType = ClickUpAuthenticationType.OAuthAccessToken;
    options.AccessToken = builder.Configuration["ClickUp:AccessToken"];
});
```

OAuth access tokens are sent as `Authorization: Bearer {token}`. ClickUp currently does not expire these tokens.

## Tasks and pagination

```csharp
var request = new GetTasksRequest
{
    Archived = false,
    Page = 0,
    IncludeClosed = true
};

var page = await clickUp.Tasks.GetTasksAsync(listId, request);

await foreach (var task in clickUp.Tasks.GetAllTasksAsync(listId, request))
{
    // page.HasMore follows ClickUp's last_page flag
}
```

List task queries return at most 100 tasks per page.

## Error handling

```csharp
try
{
    var task = await clickUp.Tasks.GetTaskAsync(taskId);
}
catch (ClickUpApiException ex)
{
    var status = ex.StatusCode;
    var code = ex.ErrorCode;
    var body = ex.ResponseBody;
    var rateLimit = ex.RateLimit;
}
```

`ClickUpApiException` includes the HTTP status, the ClickUp `ECODE` when one is present, and rate-limit headers on HTTP 429. Exception text is redacted so personal tokens, bearer tokens, client secrets, and access tokens are not written into the message.

## CancellationToken

Every network method accepts a `CancellationToken`.

```csharp
var task = await clickUp.Tasks.GetTaskAsync(taskId, cancellationToken: cancellationToken);
```

## Configuration

| Setting | Purpose |
| --- | --- |
| `PersonalToken` | Personal API token |
| `AccessToken` | OAuth access token for the default client |
| `AuthenticationType` | `PersonalToken` or `OAuthAccessToken` |
| `BaseUrl` | Defaults to `https://api.clickup.com/api/v2/` |
| `Timeout` | Per-request timeout |
| `Retry.Enabled` | Bounded retries. Defaults to `true` |
| `Retry.MaxRetries` | Extra attempts after the first request, clamped to 5 |
| `Retry.MaxDelay` | Longest wait that will still be retried |

Retries apply to HTTP 429 for every method. GET and DELETE may also retry `408`, `500`, `502`, `503`, and `504`. `Retry-After` and `X-RateLimit-Reset` are honored. If the requested wait is longer than `MaxDelay`, the call fails instead of sleeping. Cancellation still aborts a wait.

Successful responses update `IClickUpClient.LastRateLimit` from `X-RateLimit-Limit`, `X-RateLimit-Remaining`, and `X-RateLimit-Reset` when those headers are present.

Logs include the HTTP method, path, status code, duration, and retry attempts. They do not include the `Authorization` header, tokens, authorization codes, client secrets, or request bodies.

## Webhooks

Create a subscription with `clickUp.Webhooks.CreateWebhookAsync`. Store `ClickUpWebhook.Secret` securely. Verify inbound requests with the raw body and the `X-Signature` header:

```csharp
var isValid = ClickUpWebhookSignature.IsValid(rawBody, signature, webhookSecret);
var payload = ClickUpSerializer.Deserialize<ClickUpWebhookPayload>(rawBody);
```

`before` and `after` stay as JSON because their shape depends on the event.

## Custom fields

Field definitions come from `clickUp.CustomFields.GetListFieldsAsync`. Task payloads keep the raw `value` as `JsonElement`, with helpers for text, numbers, checkboxes, and dates.

```csharp
await clickUp.CustomFields.SetTaskFieldValueAsync(
    taskId,
    fieldId,
    new SetCustomFieldValueRequest
    {
        Value = ClickUpCustomFieldValue.Text("hello")
    });
```

`ClickUpCustomFieldValue` also builds number, date, checkbox, dropdown, label, user, task, progress, location, and raw JSON values.

## Testing

Unit tests use a fake `HttpMessageHandler` and do not call ClickUp.

Integration tests in `tests/ClickUp.Net.Tests/Integration` are skipped unless these environment variables are set:

```text
CLICKUP_PERSONAL_TOKEN
CLICKUP_WORKSPACE_ID
CLICKUP_TEST_LIST_ID
```

```bash
dotnet test --configuration Release
```

## Packaging

```bash
dotnet pack src/ClickUp.Net/ClickUp.Net.csproj --configuration Release
```

The package is written to `src/ClickUp.Net/bin/Release/`.

## Version

This release is `1.0.0`. Endpoint clients are separate interfaces, so additional ClickUp routes can be added without changing existing methods.
