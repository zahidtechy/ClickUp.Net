using ClickUp.Net.Endpoints.Checklists;
using ClickUp.Net.Endpoints.Comments;
using ClickUp.Net.Endpoints.CustomFields;
using ClickUp.Net.Endpoints.Folders;
using ClickUp.Net.Endpoints.Goals;
using ClickUp.Net.Endpoints.Lists;
using ClickUp.Net.Endpoints.Spaces;
using ClickUp.Net.Endpoints.Tags;
using ClickUp.Net.Endpoints.Tasks;
using ClickUp.Net.Endpoints.Teams;
using ClickUp.Net.Endpoints.TimeTracking;
using ClickUp.Net.Endpoints.Users;
using ClickUp.Net.Endpoints.Views;
using ClickUp.Net.Endpoints.Webhooks;

namespace ClickUp.Net;

/// <summary>
/// Root client for the ClickUp API v2.
/// </summary>
public interface IClickUpClient
{
    /// <summary>Gets the users endpoint.</summary>
    IUsersClient Users { get; }

    /// <summary>Gets the teams (Workspaces) endpoint.</summary>
    ITeamsClient Teams { get; }

    /// <summary>Gets the Spaces endpoint.</summary>
    ISpacesClient Spaces { get; }

    /// <summary>Gets the Folders endpoint.</summary>
    IFoldersClient Folders { get; }

    /// <summary>Gets the Lists endpoint.</summary>
    IListsClient Lists { get; }

    /// <summary>Gets the tasks endpoint.</summary>
    ITasksClient Tasks { get; }

    /// <summary>Gets the comments endpoint.</summary>
    ICommentsClient Comments { get; }

    /// <summary>Gets the checklists endpoint.</summary>
    IChecklistsClient Checklists { get; }

    /// <summary>Gets the tags endpoint.</summary>
    ITagsClient Tags { get; }

    /// <summary>Gets the views endpoint.</summary>
    IViewsClient Views { get; }

    /// <summary>Gets the webhooks endpoint.</summary>
    IWebhooksClient Webhooks { get; }

    /// <summary>Gets the time tracking endpoint.</summary>
    ITimeTrackingClient TimeTracking { get; }

    /// <summary>Gets the Goals endpoint.</summary>
    IGoalsClient Goals { get; }

    /// <summary>Gets the custom fields endpoint.</summary>
    ICustomFieldsClient CustomFields { get; }

    /// <summary>
    /// Gets rate-limit headers from the most recent response observed by this client instance.
    /// Concurrent requests can overwrite this value.
    /// </summary>
    ClickUpRateLimit? LastRateLimit { get; }
}
