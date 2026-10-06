using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A member entry returned by the Get Teams (Workspaces) endpoint.
/// </summary>
public sealed class ClickUpTeamMember
{
    /// <summary>Gets or sets the member user.</summary>
    [JsonPropertyName("user")]
    public ClickUpUser? User { get; set; }
}

/// <summary>
/// A ClickUp team, also called a Workspace.
/// </summary>
public sealed class ClickUpTeam
{
    /// <summary>Gets or sets the Workspace identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the Workspace name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the Workspace color.</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>Gets or sets the Workspace avatar URL.</summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>Gets or sets the Workspace members included in the response.</summary>
    [JsonPropertyName("members")]
    public IReadOnlyList<ClickUpTeamMember>? Members { get; set; }
}

internal sealed class ClickUpTeamsResponse
{
    [JsonPropertyName("teams")]
    public IReadOnlyList<ClickUpTeam>? Teams { get; set; }
}

internal sealed class ClickUpUserResponse
{
    [JsonPropertyName("user")]
    public ClickUpUser? User { get; set; }
}
