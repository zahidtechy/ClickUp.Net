using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A ClickUp feature that is either enabled or disabled.
/// </summary>
public sealed class ClickUpFeatureToggle
{
    /// <summary>Gets or sets a value indicating whether the feature is enabled.</summary>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }
}

/// <summary>
/// Due date settings for a Space.
/// </summary>
public sealed class ClickUpDueDatesFeature
{
    /// <summary>Gets or sets a value indicating whether due dates are enabled.</summary>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>Gets or sets a value indicating whether start dates are enabled.</summary>
    [JsonPropertyName("start_date")]
    public bool? StartDate { get; set; }

    /// <summary>Gets or sets a value indicating whether due dates are remapped when a task is moved.</summary>
    [JsonPropertyName("remap_due_dates")]
    public bool? RemapDueDates { get; set; }

    /// <summary>Gets or sets a value indicating whether closed due dates are remapped.</summary>
    [JsonPropertyName("remap_closed_due_date")]
    public bool? RemapClosedDueDate { get; set; }
}

/// <summary>
/// Feature flags configured on a ClickUp Space.
/// </summary>
public sealed class ClickUpSpaceFeatures
{
    /// <summary>Gets or sets due date settings.</summary>
    [JsonPropertyName("due_dates")]
    public ClickUpDueDatesFeature? DueDates { get; set; }

    /// <summary>Gets or sets time tracking settings.</summary>
    [JsonPropertyName("time_tracking")]
    public ClickUpFeatureToggle? TimeTracking { get; set; }

    /// <summary>Gets or sets tag settings.</summary>
    [JsonPropertyName("tags")]
    public ClickUpFeatureToggle? Tags { get; set; }

    /// <summary>Gets or sets time estimate settings.</summary>
    [JsonPropertyName("time_estimates")]
    public ClickUpFeatureToggle? TimeEstimates { get; set; }

    /// <summary>Gets or sets checklist settings.</summary>
    [JsonPropertyName("checklists")]
    public ClickUpFeatureToggle? Checklists { get; set; }

    /// <summary>Gets or sets custom field settings.</summary>
    [JsonPropertyName("custom_fields")]
    public ClickUpFeatureToggle? CustomFields { get; set; }

    /// <summary>Gets or sets dependency remapping settings.</summary>
    [JsonPropertyName("remap_dependencies")]
    public ClickUpFeatureToggle? RemapDependencies { get; set; }

    /// <summary>Gets or sets dependency warning settings.</summary>
    [JsonPropertyName("dependency_warning")]
    public ClickUpFeatureToggle? DependencyWarning { get; set; }

    /// <summary>Gets or sets portfolio settings.</summary>
    [JsonPropertyName("portfolios")]
    public ClickUpFeatureToggle? Portfolios { get; set; }

    /// <summary>Gets or sets sprint settings.</summary>
    [JsonPropertyName("sprints")]
    public ClickUpFeatureToggle? Sprints { get; set; }

    /// <summary>Gets or sets sprint point settings.</summary>
    [JsonPropertyName("points")]
    public ClickUpFeatureToggle? Points { get; set; }

    /// <summary>Gets or sets custom task type settings.</summary>
    [JsonPropertyName("custom_items")]
    public ClickUpFeatureToggle? CustomItems { get; set; }

    /// <summary>Gets or sets Zoom settings.</summary>
    [JsonPropertyName("zoom")]
    public ClickUpFeatureToggle? Zoom { get; set; }

    /// <summary>Gets or sets milestone settings.</summary>
    [JsonPropertyName("milestones")]
    public ClickUpFeatureToggle? Milestones { get; set; }

    /// <summary>Gets or sets email settings.</summary>
    [JsonPropertyName("emails")]
    public ClickUpFeatureToggle? Emails { get; set; }

    /// <summary>Gets or sets multiple-assignee settings when returned as a feature object.</summary>
    [JsonPropertyName("multiple_assignees")]
    public ClickUpFeatureToggle? MultipleAssignees { get; set; }
}
