namespace ClickUp.Net.Serialization;

/// <summary>
/// Converts between <see cref="DateTimeOffset"/> and the Unix millisecond timestamps used by ClickUp.
/// </summary>
public static class ClickUpTime
{
    /// <summary>
    /// Converts a timestamp to Unix time in milliseconds.
    /// </summary>
    /// <param name="value">The timestamp to convert.</param>
    /// <returns>The Unix timestamp in milliseconds.</returns>
    public static long ToUnixMilliseconds(DateTimeOffset value)
    {
        return value.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Converts a Unix timestamp in milliseconds to <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="milliseconds">The Unix timestamp in milliseconds.</param>
    /// <returns>The corresponding UTC timestamp.</returns>
    public static DateTimeOffset FromUnixMilliseconds(long milliseconds)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
    }
}
