namespace ClickUp.Net;

/// <summary>
/// Rate-limit information returned by the ClickUp API headers.
/// </summary>
public sealed class ClickUpRateLimit
{
    internal ClickUpRateLimit(int? limit, int? remaining, DateTimeOffset? resetsAt)
    {
        Limit = limit;
        Remaining = remaining;
        ResetsAt = resetsAt;
    }

    /// <summary>
    /// Gets the request limit for the current token, from <c>X-RateLimit-Limit</c>.
    /// </summary>
    public int? Limit { get; }

    /// <summary>
    /// Gets the number of requests remaining in the current window, from <c>X-RateLimit-Remaining</c>.
    /// </summary>
    public int? Remaining { get; }

    /// <summary>
    /// Gets the time when the current window resets, from <c>X-RateLimit-Reset</c>.
    /// </summary>
    public DateTimeOffset? ResetsAt { get; }
}
