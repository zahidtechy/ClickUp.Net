using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp Goal.
/// </summary>
public sealed class ClickUpGoal
{
    /// <summary>Gets or sets the Goal identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the pretty identifier.</summary>
    [JsonPropertyName("pretty_id")]
    public string? PrettyId { get; set; }

    /// <summary>Gets or sets the Goal name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the Workspace identifier.</summary>
    [JsonPropertyName("team_id")]
    public string? TeamId { get; set; }

    /// <summary>Gets or sets the creator user identifier.</summary>
    [JsonPropertyName("creator")]
    public int? Creator { get; set; }

    /// <summary>Gets or sets the color.</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>Gets or sets the creation time.</summary>
    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; set; }

    /// <summary>Gets or sets the start date.</summary>
    [JsonPropertyName("start_date")]
    public DateTimeOffset? StartDate { get; set; }

    /// <summary>Gets or sets the due date.</summary>
    [JsonPropertyName("due_date")]
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets a value indicating whether the Goal is private.</summary>
    [JsonPropertyName("private")]
    public bool? Private { get; set; }

    /// <summary>Gets or sets a value indicating whether the Goal is archived.</summary>
    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    /// <summary>Gets or sets a value indicating whether the Goal has multiple owners.</summary>
    [JsonPropertyName("multiple_owners")]
    public bool? MultipleOwners { get; set; }

    /// <summary>Gets or sets the percent completed.</summary>
    [JsonPropertyName("percent_completed")]
    public int? PercentCompleted { get; set; }

    /// <summary>Gets or sets the key result count, when returned by the list endpoint.</summary>
    [JsonPropertyName("key_result_count")]
    public int? KeyResultCount { get; set; }

    /// <summary>Gets or sets the pretty URL, when returned.</summary>
    [JsonPropertyName("pretty_url")]
    public string? PrettyUrl { get; set; }

    /// <summary>
    /// Gets or sets owners. List responses may return identifiers, while the Get Goal response returns user objects.
    /// </summary>
    [JsonPropertyName("owners")]
    public IReadOnlyList<JsonElement>? Owners { get; set; }
}

/// <summary>A folder of Goals.</summary>
public sealed class ClickUpGoalFolder
{
    /// <summary>Gets or sets the folder identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the folder name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the Workspace identifier.</summary>
    [JsonPropertyName("team_id")]
    public string? TeamId { get; set; }

    /// <summary>Gets or sets a value indicating whether the folder is private.</summary>
    [JsonPropertyName("private")]
    public bool? Private { get; set; }

    /// <summary>Gets or sets the number of Goals in the folder.</summary>
    [JsonPropertyName("goal_count")]
    public int? GoalCount { get; set; }

    /// <summary>Gets or sets Goals in the folder, when returned.</summary>
    [JsonPropertyName("goals")]
    public IReadOnlyList<ClickUpGoal>? Goals { get; set; }
}

/// <summary>Response from the Get Goals endpoint.</summary>
public sealed class GetGoalsResponse
{
    /// <summary>Gets or sets Goals that are not in a folder.</summary>
    [JsonPropertyName("goals")]
    public IReadOnlyList<ClickUpGoal> Goals { get; set; } = Array.Empty<ClickUpGoal>();

    /// <summary>Gets or sets Goal folders.</summary>
    [JsonPropertyName("folders")]
    public IReadOnlyList<ClickUpGoalFolder> Folders { get; set; } = Array.Empty<ClickUpGoalFolder>();
}

/// <summary>Request body for creating a Goal.</summary>
public sealed class CreateGoalRequest
{
    /// <summary>Gets or sets the Goal name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the due date.</summary>
    [JsonPropertyName("due_date")]
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Gets or sets a value indicating whether the Goal has multiple owners.</summary>
    [JsonPropertyName("multiple_owners")]
    public bool MultipleOwners { get; set; }

    /// <summary>Gets or sets owner user identifiers.</summary>
    [JsonPropertyName("owners")]
    public IReadOnlyList<int>? Owners { get; set; }

    /// <summary>Gets or sets the color.</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
}

/// <summary>Request body for updating a Goal. Unspecified properties are omitted.</summary>
[JsonConverter(typeof(OptionalPayloadConverter<UpdateGoalRequest>))]
public sealed class UpdateGoalRequest
{
    /// <summary>Gets or sets the Goal name.</summary>
    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }

    /// <summary>Gets or sets the due date.</summary>
    [JsonPropertyName("due_date")]
    public Optional<DateTimeOffset?> DueDate { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public Optional<string?> Description { get; set; }

    /// <summary>Gets or sets owner user identifiers to remove.</summary>
    [JsonPropertyName("rem_owners")]
    public Optional<IReadOnlyList<int>?> RemoveOwners { get; set; }

    /// <summary>Gets or sets owner user identifiers to add.</summary>
    [JsonPropertyName("add_owners")]
    public Optional<IReadOnlyList<int>?> AddOwners { get; set; }

    /// <summary>Gets or sets the color.</summary>
    [JsonPropertyName("color")]
    public Optional<string?> Color { get; set; }
}

internal sealed class ClickUpGoalResponse
{
    [JsonPropertyName("goal")]
    public ClickUpGoal? Goal { get; set; }
}
