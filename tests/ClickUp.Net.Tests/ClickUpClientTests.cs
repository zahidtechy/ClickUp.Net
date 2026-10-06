using System.Net;
using System.Text.Json;
using ClickUp.Net.Authentication;
using ClickUp.Net.Authentication.OAuth;
using ClickUp.Net.Exceptions;
using ClickUp.Net.Models;
using ClickUp.Net.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClickUp.Net.Tests;

public class ClickUpClientTests
{
    [Fact]
    public async Task PersonalToken_IsSentWithoutBearerScheme()
    {
        using var fixture = new ClickUpClientFixture(JsonHandler("{\"user\":{\"id\":7,\"username\":\"Ada\"}}"));

        var user = await fixture.Client.Users.GetAuthorizedUserAsync();

        Assert.Equal(7, user.Id);
        Assert.Equal("Ada", user.Username);
        Assert.Equal("pk_unit_test_token", fixture.Handler.Requests.Single().Authorization);
        Assert.Equal(HttpMethod.Get, fixture.Handler.Requests.Single().Method);
        Assert.EndsWith("/user", fixture.Handler.Requests.Single().Uri);
    }

    [Fact]
    public async Task OAuthAccessToken_IsSentAsBearer()
    {
        using var fixture = new ClickUpClientFixture(JsonHandler("{\"user\":{\"id\":1}}"));
        var client = fixture.Factory.CreateWithAccessToken("access-token-value");

        await client.Users.GetAuthorizedUserAsync();

        Assert.Equal("Bearer access-token-value", fixture.Handler.Requests.Single().Authorization);
    }

    [Fact]
    public async Task CreateTask_OmitsNullProperties_AndWritesUnixMilliseconds()
    {
        using var fixture = new ClickUpClientFixture(JsonHandler("{\"id\":\"abc\",\"name\":\"Test task\"}"));
        var due = DateTimeOffset.FromUnixTimeMilliseconds(1567780450202);

        var task = await fixture.Client.Tasks.CreateTaskAsync("list-1", new CreateTaskRequest
        {
            Name = "Test task",
            Description = null,
            DueDate = due,
            NotifyAll = false,
            Priority = ClickUpTaskPriorities.High
        });

        Assert.Equal("abc", task.Id);
        var body = fixture.Handler.Requests.Single().Body!;
        using var document = JsonDocument.Parse(body);
        Assert.Equal("Test task", document.RootElement.GetProperty("name").GetString());
        Assert.False(document.RootElement.TryGetProperty("description", out _));
        Assert.Equal(1567780450202, document.RootElement.GetProperty("due_date").GetInt64());
        Assert.False(document.RootElement.GetProperty("notify_all").GetBoolean());
        Assert.Equal(2, document.RootElement.GetProperty("priority").GetInt32());
        Assert.Contains("/list/list-1/task", fixture.Handler.Requests.Single().Uri);
    }

    [Fact]
    public async Task UpdateTask_OmitsUnspecifiedProperties_AndWritesExplicitNull()
    {
        using var fixture = new ClickUpClientFixture(JsonHandler("{\"id\":\"abc\",\"name\":\"Task\"}"));

        await fixture.Client.Tasks.UpdateTaskAsync("abc", new UpdateTaskRequest
        {
            Status = "closed",
            Priority = null
        });

        using var document = JsonDocument.Parse(fixture.Handler.Requests.Single().Body!);
        Assert.Equal("closed", document.RootElement.GetProperty("status").GetString());
        Assert.Equal(JsonValueKind.Null, document.RootElement.GetProperty("priority").ValueKind);
        Assert.False(document.RootElement.TryGetProperty("name", out _));
        Assert.False(document.RootElement.TryGetProperty("assignees", out _));
    }

    [Fact]
    public async Task GetTasks_WritesFiltersAndRepeatedQueryParameters()
    {
        using var fixture = new ClickUpClientFixture(JsonHandler("{\"tasks\":[],\"last_page\":true}"));
        var due = DateTimeOffset.FromUnixTimeMilliseconds(1700000000000);

        var page = await fixture.Client.Tasks.GetTasksAsync("99", new GetTasksRequest
        {
            Archived = false,
            Page = 0,
            IncludeClosed = true,
            Statuses = new[] { "open", "in progress" },
            DueDateGreaterThan = due,
            CustomFields = new[]
            {
                new ClickUpCustomFieldFilter
                {
                    FieldId = "field-1",
                    Operator = ClickUpCustomFieldOperators.Equal,
                    Value = JsonSerializer.SerializeToElement("blue")
                }
            }
        });

        Assert.True(page.LastPage);
        Assert.False(page.HasMore);
        var uri = Uri.UnescapeDataString(fixture.Handler.Requests.Single().Uri);
        Assert.Contains("archived=false", uri);
        Assert.Contains("page=0", uri);
        Assert.Contains("include_closed=true", uri);
        Assert.Contains("statuses=open", uri);
        Assert.Contains("statuses=in progress", uri);
        Assert.Contains("due_date_gt=1700000000000", uri);
        Assert.Contains("\"field_id\":\"field-1\"", uri);
        Assert.Contains("\"operator\":\"=\"", uri);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task ErrorResponses_ExposeStatusAndClickUpCode(HttpStatusCode statusCode)
    {
        var handler = new RecordingHandler
        {
            Responder = (_, _) => Error(statusCode, "{\"err\":\"Token invalid pk_secret_value\",\"ECODE\":\"OAUTH_025\"}", statusCode == HttpStatusCode.TooManyRequests)
        };
        using var fixture = new ClickUpClientFixture(handler);

        var exception = await Assert.ThrowsAsync<ClickUpApiException>(() => fixture.Client.Tasks.GetTaskAsync("missing"));

        Assert.Equal(statusCode, exception.StatusCode);
        Assert.Equal("OAUTH_025", exception.ErrorCode);
        Assert.DoesNotContain("pk_secret_value", exception.Message);
        Assert.DoesNotContain("pk_secret_value", exception.ResponseBody);
        Assert.Contains("pk_[redacted]", exception.ResponseBody);
        if (statusCode == HttpStatusCode.TooManyRequests)
        {
            Assert.Equal(100, exception.RateLimit?.Limit);
            Assert.Equal(0, exception.RateLimit?.Remaining);
            Assert.NotNull(exception.RateLimit?.ResetsAt);
        }
    }

    [Fact]
    public async Task CancellationToken_CancelsTheRequest()
    {
        var handler = new RecordingHandler
        {
            BeforeResponse = cancellationToken => Task.Delay(Timeout.Infinite, cancellationToken)
        };
        using var fixture = new ClickUpClientFixture(handler);
        using var source = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => fixture.Client.Users.GetAuthorizedUserAsync(source.Token));
    }

    [Fact]
    public async Task GetAllTasks_FollowsLastPage()
    {
        var handler = new RecordingHandler
        {
            Responder = (request, _) =>
            {
                var lastPage = request.Uri.Contains("page=1", StringComparison.Ordinal);
                var id = lastPage ? "second" : "first";
                return Json("{\"tasks\":[{\"id\":\"" + id + "\"}],\"last_page\":" + (lastPage ? "true" : "false") + "}");
            }
        };
        using var fixture = new ClickUpClientFixture(handler);

        var ids = new List<string?>();
        await foreach (var task in fixture.Client.Tasks.GetAllTasksAsync("list-1", new GetTasksRequest { IncludeClosed = true }))
        {
            ids.Add(task.Id);
        }

        Assert.Equal(new[] { "first", "second" }, ids);
        Assert.Contains("page=0", fixture.Handler.Requests[0].Uri);
        Assert.Contains("page=1", fixture.Handler.Requests[1].Uri);
        Assert.Contains("include_closed=true", fixture.Handler.Requests[0].Uri);
    }

    [Fact]
    public void UnixTimestamps_RoundTripThroughTheSerializer()
    {
        var due = DateTimeOffset.FromUnixTimeMilliseconds(1567780450202);
        var json = ClickUpSerializer.Serialize(new CreateTaskRequest
        {
            Name = "Dated",
            DueDate = due
        });

        var restored = ClickUpSerializer.Deserialize<CreateTaskRequest>(json);
        Assert.NotNull(restored);
        Assert.Equal(due, restored!.DueDate);

        var task = ClickUpSerializer.Deserialize<ClickUpTask>("{\"id\":\"t\",\"date_created\":\"1567780450202\",\"time_estimate\":\"3600\"}");
        Assert.Equal(due, task!.DateCreated);
        Assert.Equal(3600, task.TimeEstimate);
    }

    [Fact]
    public async Task OAuthExchange_PostsCredentialsWithoutAuthorizationHeader()
    {
        var handler = new RecordingHandler
        {
            Responder = (_, _) => Json("{\"access_token\":\"oauth-access-token\"}")
        };
        using var fixture = new ClickUpClientFixture(handler, logging: true);

        var url = fixture.OAuth.GetAuthorizationUrl("client id", "https://app.example/callback", "state value");
        Assert.Contains("client_id=client%20id", url);
        Assert.Contains("redirect_uri=https%3A%2F%2Fapp.example%2Fcallback", url);
        Assert.Contains("state=state%20value", url);

        var token = await fixture.OAuth.ExchangeCodeForTokenAsync("client-id", "super-secret", "auth-code");

        Assert.Equal("oauth-access-token", token.AccessToken);
        var request = fixture.Handler.Requests.Single();
        Assert.Null(request.Authorization);
        Assert.Contains("/oauth/token", request.Uri);
        Assert.Contains("\"client_id\":\"client-id\"", request.Body);
        Assert.Contains("\"client_secret\":\"super-secret\"", request.Body);
        Assert.Contains("\"code\":\"auth-code\"", request.Body);
        var logs = string.Join("\n", fixture.Logger!.Messages);
        Assert.DoesNotContain("super-secret", logs);
        Assert.DoesNotContain("auth-code", logs);
        Assert.DoesNotContain("oauth-access-token", logs);
        Assert.DoesNotContain("pk_unit_test_token", logs);
    }

    [Fact]
    public async Task Logs_DoNotContainThePersonalToken()
    {
        using var fixture = new ClickUpClientFixture(JsonHandler("{\"user\":{\"id\":1}}"), logging: true);

        await fixture.Client.Users.GetAuthorizedUserAsync();

        var logs = string.Join("\n", fixture.Logger!.Messages);
        Assert.DoesNotContain("pk_unit_test_token", logs);
        Assert.Contains("GET", logs);
        Assert.Contains("200", logs);
    }

    [Fact]
    public async Task RateLimitHeaders_AreExposedAfterASuccessfulCall()
    {
        var handler = new RecordingHandler
        {
            Responder = (_, _) =>
            {
                var response = Json("{\"teams\":[{\"id\":\"42\",\"name\":\"Workspace\"}]}");
                response.Headers.TryAddWithoutValidation("X-RateLimit-Limit", "100");
                response.Headers.TryAddWithoutValidation("X-RateLimit-Remaining", "99");
                response.Headers.TryAddWithoutValidation("X-RateLimit-Reset", "1700000000");
                return response;
            }
        };
        using var fixture = new ClickUpClientFixture(handler);

        var teams = await fixture.Client.Teams.GetTeamsAsync();

        Assert.Equal("42", teams[0].Id);
        Assert.Equal(99, fixture.Client.LastRateLimit?.Remaining);
    }

    [Fact]
    public async Task Retry_Retries429ThenSucceeds()
    {
        var attempts = 0;
        var handler = new RecordingHandler
        {
            Responder = (_, _) =>
            {
                attempts++;
                if (attempts == 1)
                {
                    var response = Error(HttpStatusCode.TooManyRequests, "{\"err\":\"Rate limit reached\",\"ECODE\":\"RATE_001\"}", includeRateLimit: true);
                    response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.Zero);
                    return response;
                }

                return Json("{\"user\":{\"id\":3,\"timezone\":\"America/Los_Angeles\"}}");
            }
        };
        using var fixture = new ClickUpClientFixture(handler, options =>
        {
            options.Retry.Enabled = true;
            options.Retry.MaxRetries = 2;
        });

        var user = await fixture.Client.Users.GetAuthorizedUserAsync();

        Assert.Equal(3, user.Id);
        Assert.Equal("America/Los_Angeles", user.Timezone);
        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task CustomFieldDateValue_WritesValueOptions()
    {
        using var fixture = new ClickUpClientFixture(JsonHandler("{}"));

        await fixture.Client.CustomFields.SetTaskFieldValueAsync("task", "field", new SetCustomFieldValueRequest
        {
            Value = ClickUpCustomFieldValue.Date(DateTimeOffset.FromUnixTimeMilliseconds(1567780450202), includeTime: true)
        });

        using var document = JsonDocument.Parse(fixture.Handler.Requests.Single().Body!);
        Assert.Equal(1567780450202, document.RootElement.GetProperty("value").GetInt64());
        Assert.True(document.RootElement.GetProperty("value_options").GetProperty("time").GetBoolean());
    }

    [Fact]
    public void WebhookSignature_MatchesHexHmacSha256()
    {
        const string body = "{\"webhook_id\":\"abc\",\"event\":\"taskCreated\",\"task_id\":\"c0j\"}";
        const string secret = "webhook-secret";
        using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret));
        var signature = ToHex(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(body)));

        Assert.True(ClickUpWebhookSignature.IsValid(body, signature, secret));
        Assert.False(ClickUpWebhookSignature.IsValid(body, signature, "other-secret"));
        Assert.False(ClickUpWebhookSignature.IsValid(body + " ", signature, secret));

        var payload = ClickUpSerializer.Deserialize<ClickUpWebhookPayload>(body);
        Assert.Equal(ClickUpWebhookEvents.TaskCreated, payload!.Event);
        Assert.Equal("c0j", payload.TaskId);
    }

    [Fact]
    public void ConfigurationSection_BindsPersonalToken()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ClickUp:PersonalToken"] = "pk_from_config",
                ["ClickUp:BaseUrl"] = "https://api.clickup.com/api/v2/"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddClickUp(configuration.GetSection("ClickUp"));
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ClickUpOptions>>().Value;

        Assert.Equal("pk_from_config", options.PersonalToken);
        Assert.Equal(ClickUpAuthenticationType.PersonalToken, options.AuthenticationType);
        Assert.StartsWith("https://api.clickup.com/api/v2/", options.BaseUrl);
    }

    private static RecordingHandler JsonHandler(string json)
    {
        return new RecordingHandler
        {
            Responder = (_, _) => Json(json)
        };
    }

    private static HttpResponseMessage Json(string json)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
    }

    private static HttpResponseMessage Error(HttpStatusCode statusCode, string json, bool includeRateLimit)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        if (includeRateLimit)
        {
            response.Headers.TryAddWithoutValidation("X-RateLimit-Limit", "100");
            response.Headers.TryAddWithoutValidation("X-RateLimit-Remaining", "0");
            response.Headers.TryAddWithoutValidation("X-RateLimit-Reset", "1700000000");
        }

        return response;
    }

    private static string ToHex(byte[] bytes)
    {
        var characters = new char[bytes.Length * 2];
        var index = 0;
        foreach (var value in bytes)
        {
            characters[index++] = (char)((value >> 4) < 10 ? '0' + (value >> 4) : 'a' + ((value >> 4) - 10));
            characters[index++] = (char)((value & 0xF) < 10 ? '0' + (value & 0xF) : 'a' + ((value & 0xF) - 10));
        }

        return new string(characters);
    }
}
