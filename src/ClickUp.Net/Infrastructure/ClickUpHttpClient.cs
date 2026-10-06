using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ClickUp.Net;
using ClickUp.Net.Authentication;
using ClickUp.Net.Exceptions;
using ClickUp.Net.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClickUp.Net.Infrastructure;

internal sealed class ClickUpHttpClient : IClickUpHttpClient
{
    internal const string HttpClientName = "ClickUp.Net";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IClickUpAuthenticator? _authenticator;
    private readonly ClickUpOptions _options;
    private readonly ILogger _logger;
    private ClickUpRateLimit? _lastRateLimit;

    public ClickUpHttpClient(
        IHttpClientFactory httpClientFactory,
        IClickUpAuthenticator? authenticator,
        IOptions<ClickUpOptions> options,
        ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _authenticator = authenticator;
        _options = options.Value;
        _logger = logger;
    }

    public ClickUpRateLimit? LastRateLimit => Volatile.Read(ref _lastRateLimit);

    public Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Get, endpoint, jsonBody: null, authenticate: true, cancellationToken);
    }

    public Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Post, endpoint, Serialize(request), authenticate: true, cancellationToken);
    }

    public async Task PostAsync<TRequest>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        await SendAsync<JsonElement>(HttpMethod.Post, endpoint, Serialize(request), authenticate: true, cancellationToken).ConfigureAwait(false);
    }

    public Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
    {
        return SendAsync<TResponse>(HttpMethod.Put, endpoint, Serialize(request), authenticate: true, cancellationToken);
    }

    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        await SendAsync<JsonElement>(HttpMethod.Delete, endpoint, jsonBody: null, authenticate: true, cancellationToken).ConfigureAwait(false);
    }

    internal Task<TResponse?> SendWithoutAuthenticationAsync<TResponse>(
        HttpMethod method,
        string endpoint,
        string? jsonBody,
        CancellationToken cancellationToken)
    {
        return SendAsync<TResponse>(method, endpoint, jsonBody, authenticate: false, cancellationToken);
    }

    private static string? Serialize<TRequest>(TRequest request)
    {
        if (request is null)
        {
            return null;
        }

        return JsonSerializer.Serialize(request, ClickUpSerializer.Options);
    }

    private async Task<TResponse?> SendAsync<TResponse>(
        HttpMethod method,
        string endpoint,
        string? jsonBody,
        bool authenticate,
        CancellationToken cancellationToken)
    {
        var maxRetries = _options.Retry.Enabled ? Math.Max(0, Math.Min(_options.Retry.MaxRetries, 5)) : 0;
        var attempt = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            attempt++;

            using var request = new HttpRequestMessage(method, endpoint);
            if (authenticate)
            {
                if (_authenticator is null)
                {
                    throw new InvalidOperationException("ClickUp authentication is not configured.");
                }

                request.Headers.TryAddWithoutValidation("Authorization", _authenticator.CreateAuthorizationHeader());
            }

            if (jsonBody is not null)
            {
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }
            else if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Delete)
            {
                request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            }

            var httpClient = _httpClientFactory.CreateClient(HttpClientName);
            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException) when (IsIdempotent(method) && attempt <= maxRetries)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                if (delay > _options.Retry.MaxDelay)
                {
                    throw;
                }

                _logger.LogWarning(
                    "ClickUp {Method} {Path} failed before a response. Retry {Attempt} of {MaxRetries} after {DelayMs} ms.",
                    method.Method,
                    PathOnly(endpoint),
                    attempt,
                    maxRetries,
                    (int)delay.TotalMilliseconds);
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                continue;
            }

            using (response)
            {
                stopwatch.Stop();
                var rateLimit = ReadRateLimit(response.Headers);
                if (rateLimit is not null)
                {
                    Volatile.Write(ref _lastRateLimit, rateLimit);
                }

                _logger.LogInformation(
                    "ClickUp {Method} {Path} responded {StatusCode} in {DurationMs} ms.",
                    method.Method,
                    PathOnly(endpoint),
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                if (!response.IsSuccessStatusCode &&
                    IsRetryable(method, response.StatusCode) &&
                    attempt <= maxRetries &&
                    TryGetRetryDelay(response, attempt, out var retryDelay))
                {
                    _logger.LogWarning(
                        "ClickUp {Method} {Path} returned {StatusCode}. Retry {Attempt} of {MaxRetries} after {DelayMs} ms.",
                        method.Method,
                        PathOnly(endpoint),
                        (int)response.StatusCode,
                        attempt,
                        maxRetries,
                        (int)retryDelay.TotalMilliseconds);
                    await Task.Delay(retryDelay, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                var body = response.Content is null
                    ? string.Empty
                    : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw ClickUpApiException.FromResponse(response.StatusCode, body, rateLimit, response.Headers);
                }

                if (string.IsNullOrWhiteSpace(body))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<TResponse>(body, ClickUpSerializer.Options);
            }
        }
    }

    private bool TryGetRetryDelay(HttpResponseMessage response, int attempt, out TimeSpan delay)
    {
        delay = TimeSpan.Zero;
        var maxDelay = _options.Retry.MaxDelay <= TimeSpan.Zero
            ? TimeSpan.FromSeconds(30)
            : _options.Retry.MaxDelay;

        if (response.Headers.RetryAfter is { } retryAfter)
        {
            if (retryAfter.Delta is { } delta)
            {
                return AcceptDelay(delta, maxDelay, out delay);
            }

            if (retryAfter.Date is { } date)
            {
                var wait = date - DateTimeOffset.UtcNow;
                if (wait < TimeSpan.Zero)
                {
                    wait = TimeSpan.Zero;
                }

                return AcceptDelay(wait, maxDelay, out delay);
            }
        }

        if (response.Headers.TryGetValues("X-RateLimit-Reset", out var resetValues))
        {
            var raw = resetValues.FirstOrDefault();
            if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var reset))
            {
                var resetsAt = reset > 9999999999L
                    ? DateTimeOffset.FromUnixTimeMilliseconds(reset)
                    : DateTimeOffset.FromUnixTimeSeconds(reset);
                var wait = resetsAt - DateTimeOffset.UtcNow;
                if (wait < TimeSpan.Zero)
                {
                    wait = TimeSpan.FromSeconds(1);
                }

                return AcceptDelay(wait, maxDelay, out delay);
            }
        }

        var backoff = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
        return AcceptDelay(backoff, maxDelay, out delay);
    }

    private static bool AcceptDelay(TimeSpan candidate, TimeSpan maxDelay, out TimeSpan delay)
    {
        if (candidate < TimeSpan.Zero)
        {
            candidate = TimeSpan.Zero;
        }

        if (candidate > maxDelay)
        {
            delay = TimeSpan.Zero;
            return false;
        }

        delay = candidate;
        return true;
    }

    private static bool IsRetryable(HttpMethod method, HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.TooManyRequests)
        {
            return true;
        }

        if (!IsIdempotent(method))
        {
            return false;
        }

        var code = (int)statusCode;
        return code == 408 || code == 500 || code == 502 || code == 503 || code == 504;
    }

    private static bool IsIdempotent(HttpMethod method)
    {
        return method == HttpMethod.Get || method == HttpMethod.Delete;
    }

    internal static ClickUpRateLimit? ReadRateLimit(HttpResponseHeaders headers)
    {
        var limit = ReadInt(headers, "X-RateLimit-Limit");
        var remaining = ReadInt(headers, "X-RateLimit-Remaining");
        DateTimeOffset? resetsAt = null;
        if (headers.TryGetValues("X-RateLimit-Reset", out var values) &&
            long.TryParse(values.FirstOrDefault(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var reset))
        {
            resetsAt = reset > 9999999999L
                ? DateTimeOffset.FromUnixTimeMilliseconds(reset)
                : DateTimeOffset.FromUnixTimeSeconds(reset);
        }

        if (limit is null && remaining is null && resetsAt is null)
        {
            return null;
        }

        return new ClickUpRateLimit(limit, remaining, resetsAt);
    }

    private static int? ReadInt(HttpResponseHeaders headers, string name)
    {
        if (!headers.TryGetValues(name, out var values))
        {
            return null;
        }

        return int.TryParse(values.FirstOrDefault(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : null;
    }

    private static string PathOnly(string endpoint)
    {
        var queryIndex = endpoint.IndexOf('?', StringComparison.Ordinal);
        return queryIndex < 0 ? endpoint : endpoint.Substring(0, queryIndex);
    }
}
