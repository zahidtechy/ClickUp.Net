using ClickUp.Net.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ClickUp.Net.Tests.Integration;

public class ClickUpIntegrationTests
{
    [ClickUpIntegrationFact]
    public async Task AuthorizedUserTeamsAndTasks_CanBeRead()
    {
        var token = Environment.GetEnvironmentVariable("CLICKUP_PERSONAL_TOKEN");
        var workspaceId = Environment.GetEnvironmentVariable("CLICKUP_WORKSPACE_ID");
        var listId = Environment.GetEnvironmentVariable("CLICKUP_TEST_LIST_ID");

        var services = new ServiceCollection();
        services.AddClickUp(options =>
        {
            options.PersonalToken = token;
            options.Retry.Enabled = false;
        });

        await using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IClickUpClient>();

        var user = await client.Users.GetAuthorizedUserAsync();
        Assert.False(string.IsNullOrWhiteSpace(user.Username) && user.Id is null);

        var teams = await client.Teams.GetTeamsAsync();
        Assert.Contains(teams, team => team.Id == workspaceId);

        var tasks = await client.Tasks.GetTasksAsync(listId!, new GetTasksRequest
        {
            Page = 0,
            IncludeClosed = true
        });
        Assert.NotNull(tasks.Tasks);
    }
}

public sealed class ClickUpIntegrationFactAttribute : FactAttribute
{
    public ClickUpIntegrationFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CLICKUP_PERSONAL_TOKEN")) ||
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CLICKUP_WORKSPACE_ID")) ||
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CLICKUP_TEST_LIST_ID")))
        {
            Skip = "Set CLICKUP_PERSONAL_TOKEN, CLICKUP_WORKSPACE_ID, and CLICKUP_TEST_LIST_ID to run ClickUp integration tests.";
        }
    }
}
