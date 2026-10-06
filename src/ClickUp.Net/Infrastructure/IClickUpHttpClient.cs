using ClickUp.Net;

namespace ClickUp.Net.Infrastructure;

internal interface IClickUpHttpClient
{
    ClickUpRateLimit? LastRateLimit { get; }

    Task<TResponse?> GetAsync<TResponse>(
        string endpoint,
        CancellationToken cancellationToken = default);

    Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default);

    Task PostAsync<TRequest>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default);

    Task<TResponse?> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string endpoint,
        CancellationToken cancellationToken = default);
}
