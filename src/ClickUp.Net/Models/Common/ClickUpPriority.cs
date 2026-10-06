using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp priority value returned on a task or list.
/// </summary>
public sealed class ClickUpPriority
{
    /// <summary>Gets or sets the priority color.</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>Gets or sets the priority identifier. <c>1</c> is urgent, <c>2</c> is high, <c>3</c> is normal, and <c>4</c> is low.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the priority order index.</summary>
    [JsonPropertyName("orderindex")]
    public string? OrderIndex { get; set; }

    /// <summary>Gets or sets the priority label.</summary>
    [JsonPropertyName("priority")]
    public string? Priority { get; set; }
}

/// <summary>
/// Priority identifiers accepted by ClickUp task create and update requests.
/// </summary>
public static class ClickUpTaskPriorities
{
    /// <summary>Urgent priority.</summary>
    public const int Urgent = 1;

    /// <summary>High priority.</summary>
    public const int High = 2;

    /// <summary>Normal priority.</summary>
    public const int Normal = 3;

    /// <summary>Low priority.</summary>
    public const int Low = 4;
}
