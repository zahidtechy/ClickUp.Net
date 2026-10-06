using ClickUp.Net.Infrastructure;
using ClickUp.Net.Models;

namespace ClickUp.Net.Endpoints.Webhooks;

/// <summary>
/// Manages ClickUp webhook registrations.
/// </summary>
public interface IWebhooksClient
{
    /// <summary>Gets webhooks in a Workspace.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<ClickUpWebhook>> GetWebhooksAsync(string teamId, CancellationToken cancellationToken = default);

    /// <summary>Creates a webhook.</summary>
    /// <param name="teamId">The Workspace identifier.</param>
    /// <param name="request">The webhook to create.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpWebhook> CreateWebhookAsync(string teamId, CreateWebhookRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates a webhook.</summary>
    /// <param name="webhookId">The webhook identifier.</param>
    /// <param name="request">The fields to update.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<ClickUpWebhook> UpdateWebhookAsync(string webhookId, UpdateWebhookRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes a webhook.</summary>
    /// <param name="webhookId">The webhook identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteWebhookAsync(string webhookId, CancellationToken cancellationToken = default);
}

internal sealed class WebhooksClient : IWebhooksClient
{
    private readonly IClickUpHttpClient _http;

    public WebhooksClient(IClickUpHttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<ClickUpWebhook>> GetWebhooksAsync(string teamId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync<ClickUpWebhooksResponse>("team/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(teamId, nameof(teamId))) + "/webhook", cancellationToken).ConfigureAwait(false);
        return ApiResults.List(response?.Webhooks);
    }

    public async Task<ClickUpWebhook> CreateWebhookAsync(string teamId, CreateWebhookRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(teamId, nameof(teamId));
        Guard.NotNull(request, nameof(request));
        Guard.NotNullOrWhiteSpace(request.Endpoint, nameof(request.Endpoint));
        if (request.Events is null || request.Events.Count == 0)
        {
            throw new ArgumentException("At least one webhook event is required.", nameof(request));
        }

        var response = await _http.PostAsync<CreateWebhookRequest, ClickUpWebhookResponse>(
            "team/" + Uri.EscapeDataString(teamId) + "/webhook",
            request,
            cancellationToken).ConfigureAwait(false);
        var webhook = ApiResults.Required(response?.Webhook, "webhook");
        webhook.Id ??= response!.Id;
        return webhook;
    }

    public async Task<ClickUpWebhook> UpdateWebhookAsync(string webhookId, UpdateWebhookRequest request, CancellationToken cancellationToken = default)
    {
        Guard.NotNullOrWhiteSpace(webhookId, nameof(webhookId));
        Guard.NotNull(request, nameof(request));
        var response = await _http.PutAsync<UpdateWebhookRequest, ClickUpWebhookResponse>("webhook/" + Uri.EscapeDataString(webhookId), request, cancellationToken).ConfigureAwait(false);
        var webhook = ApiResults.Required(response?.Webhook, "webhook");
        webhook.Id ??= response!.Id;
        return webhook;
    }

    public Task DeleteWebhookAsync(string webhookId, CancellationToken cancellationToken = default)
    {
        return _http.DeleteAsync("webhook/" + Uri.EscapeDataString(Guard.NotNullOrWhiteSpace(webhookId, nameof(webhookId))), cancellationToken);
    }
}
