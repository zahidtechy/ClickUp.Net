using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Net.Infrastructure;
using ClickUpRateLimit = ClickUp.Net.ClickUpRateLimit;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Exceptions;

/// <summary>
/// The exception thrown when the ClickUp API returns an unsuccessful HTTP status code.
/// </summary>
public class ClickUpApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClickUpApiException"/> class.
    /// </summary>
    /// <param name="message">The exception message. Secrets are removed before this value is stored.</param>
    /// <param name="statusCode">The HTTP status code returned by ClickUp.</param>
    /// <param name="errorCode">The ClickUp error code, when one was present.</param>
    /// <param name="responseBody">The response body with secrets removed.</param>
    /// <param name="requestId">The request identifier header, when ClickUp or a proxy sent one.</param>
    /// <param name="rateLimit">Rate-limit headers from the response, when present.</param>
    public ClickUpApiException(
        string message,
        HttpStatusCode statusCode,
        string? errorCode,
        string? responseBody,
        string? requestId,
        ClickUpRateLimit? rateLimit)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ResponseBody = responseBody;
        RequestId = requestId;
        RateLimit = rateLimit;
    }

    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the ClickUp error code, such as <c>OAUTH_017</c>, when the body included one.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the response body with credential-like values removed.
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Gets the value of <c>X-Request-Id</c> when that header is present.
    /// </summary>
    public string? RequestId { get; }

    /// <summary>
    /// Gets rate-limit information when the response included ClickUp rate-limit headers.
    /// </summary>
    public ClickUpRateLimit? RateLimit { get; }

    internal static ClickUpApiException FromResponse(
        HttpStatusCode statusCode,
        string? responseBody,
        ClickUpRateLimit? rateLimit,
        HttpResponseHeaders headers)
    {
        var boundedBody = responseBody ?? string.Empty;
        if (boundedBody.Length > 8192)
        {
            boundedBody = boundedBody.Substring(0, 8192);
        }

        var redactedBody = SecretRedactor.Redact(boundedBody);
        string? errorCode = null;
        string? errorMessage = null;

        if (!string.IsNullOrWhiteSpace(boundedBody))
        {
            try
            {
                var payload = JsonSerializer.Deserialize<ClickUpErrorPayload>(boundedBody, ClickUpSerializer.Options);
                errorCode = FirstNonEmpty(payload?.ECode, payload?.Code);
                errorMessage = FirstNonEmpty(payload?.Err, payload?.Error, payload?.Message);
            }
            catch (JsonException)
            {
                errorMessage = null;
            }
        }

        var message = string.IsNullOrWhiteSpace(errorMessage)
            ? $"ClickUp API request failed with status {(int)statusCode} ({statusCode})."
            : $"ClickUp API request failed with status {(int)statusCode} ({statusCode}): {SecretRedactor.Redact(errorMessage)}";

        if (!string.IsNullOrWhiteSpace(errorCode))
        {
            message += $" ({errorCode})";
        }

        string? requestId = null;
        if (headers.TryGetValues("X-Request-Id", out var requestIds))
        {
            requestId = requestIds.FirstOrDefault();
        }

        return new ClickUpApiException(message, statusCode, errorCode, redactedBody, requestId, rateLimit);
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private sealed class ClickUpErrorPayload
    {
        [JsonPropertyName("err")]
        public string? Err { get; set; }

        [JsonPropertyName("ECODE")]
        public string? ECode { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
