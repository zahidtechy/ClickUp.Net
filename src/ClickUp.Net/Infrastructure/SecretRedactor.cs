using System.Text.RegularExpressions;

namespace ClickUp.Net.Infrastructure;

internal static class SecretRedactor
{
    private static readonly Regex PersonalToken = new(@"pk_[A-Za-z0-9_\-]+", RegexOptions.Compiled);
    private static readonly Regex BearerToken = new(@"Bearer\s+\S+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex JsonSecret = new(
        "(?i)(\"(?:access_token|client_secret|client_id|code|secret|personal_token|authorization)\"\\s*:\\s*\")[^\"]*(\")",
        RegexOptions.Compiled);

    public static string Redact(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var redacted = PersonalToken.Replace(value, "pk_[redacted]");
        redacted = BearerToken.Replace(redacted, "Bearer [redacted]");
        redacted = JsonSecret.Replace(redacted, "$1[redacted]$2");
        return redacted;
    }
}
