using System.Security.Cryptography;
using System.Text;

namespace ClickUp.Net.Models;

/// <summary>
/// Verifies the <c>X-Signature</c> header ClickUp sends with webhook requests.
/// The signature is the hexadecimal HMAC-SHA256 of the raw request body using the webhook secret.
/// </summary>
public static class ClickUpWebhookSignature
{
    /// <summary>The HTTP header that carries the signature.</summary>
    public const string HeaderName = "X-Signature";

    /// <summary>
    /// Checks a raw request body against the signature header.
    /// </summary>
    /// <param name="rawBody">The exact request body bytes ClickUp sent.</param>
    /// <param name="signature">The <c>X-Signature</c> header value.</param>
    /// <param name="secret">The webhook secret returned when the webhook was created.</param>
    /// <returns><see langword="true"/> when the signature matches.</returns>
    public static bool IsValid(ReadOnlySpan<byte> rawBody, string? signature, string? secret)
    {
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secret))
        {
            return false;
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(rawBody.ToArray());
        var expected = ToHex(hash);
        var provided = signature.Trim();
        var expectedBytes = Encoding.ASCII.GetBytes(expected);
        var providedBytes = Encoding.ASCII.GetBytes(provided.ToLowerInvariant());
        if (expectedBytes.Length != providedBytes.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }

    /// <summary>
    /// Checks a raw UTF-8 request body against the signature header.
    /// </summary>
    /// <param name="rawBody">The exact request body text ClickUp sent.</param>
    /// <param name="signature">The <c>X-Signature</c> header value.</param>
    /// <param name="secret">The webhook secret returned when the webhook was created.</param>
    /// <returns><see langword="true"/> when the signature matches.</returns>
    public static bool IsValid(string rawBody, string? signature, string? secret)
    {
        return IsValid(Encoding.UTF8.GetBytes(rawBody ?? string.Empty), signature, secret);
    }

    private static string ToHex(byte[] bytes)
    {
        var characters = new char[bytes.Length * 2];
        var index = 0;
        foreach (var value in bytes)
        {
            characters[index++] = GetHex(value >> 4);
            characters[index++] = GetHex(value & 0xF);
        }

        return new string(characters);
    }

    private static char GetHex(int value)
    {
        return (char)(value < 10 ? '0' + value : 'a' + (value - 10));
    }
}
