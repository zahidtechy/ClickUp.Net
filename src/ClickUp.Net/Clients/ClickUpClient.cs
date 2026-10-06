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
using ClickUp.Net.Infrastructure;

namespace ClickUp.Net;

/// <summary>
/// Default <see cref="IClickUpClient"/> implementation.
/// </summary>
public sealed class ClickUpClient : IClickUpClient
{
    private readonly IClickUpHttpClient _http;

    internal ClickUpClient(IClickUpHttpClient http)
    {
        _http = http;
        Users = new UsersClient(http);
        Teams = new TeamsClient(http);
        Spaces = new SpacesClient(http);
        Folders = new FoldersClient(http);
        Lists = new ListsClient(http);
        Tasks = new TasksClient(http);
        Comments = new CommentsClient(http);
        Checklists = new ChecklistsClient(http);
        Tags = new TagsClient(http);
        Views = new ViewsClient(http);
        Webhooks = new WebhooksClient(http);
        TimeTracking = new TimeTrackingClient(http);
        Goals = new GoalsClient(http);
        CustomFields = new CustomFieldsClient(http);
    }

    /// <inheritdoc />
    public IUsersClient Users { get; }

    /// <inheritdoc />
    public ITeamsClient Teams { get; }

    /// <inheritdoc />
    public ISpacesClient Spaces { get; }

    /// <inheritdoc />
    public IFoldersClient Folders { get; }

    /// <inheritdoc />
    public IListsClient Lists { get; }

    /// <inheritdoc />
    public ITasksClient Tasks { get; }

    /// <inheritdoc />
    public ICommentsClient Comments { get; }

    /// <inheritdoc />
    public IChecklistsClient Checklists { get; }

    /// <inheritdoc />
    public ITagsClient Tags { get; }

    /// <inheritdoc />
    public IViewsClient Views { get; }

    /// <inheritdoc />
    public IWebhooksClient Webhooks { get; }

    /// <inheritdoc />
    public ITimeTrackingClient TimeTracking { get; }

    /// <inheritdoc />
    public IGoalsClient Goals { get; }

    /// <inheritdoc />
    public ICustomFieldsClient CustomFields { get; }

    /// <inheritdoc />
    public ClickUpRateLimit? LastRateLimit => _http.LastRateLimit;
}
