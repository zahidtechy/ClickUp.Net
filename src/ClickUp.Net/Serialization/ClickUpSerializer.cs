using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Net.Serialization;

/// <summary>
/// Shared System.Text.Json settings used by the ClickUp client and by applications deserializing webhook payloads.
/// </summary>
public static class ClickUpSerializer
{
    /// <summary>
    /// Gets the serializer options used for ClickUp request and response payloads.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = CreateOptions();

    /// <summary>
    /// Serializes a value with the ClickUp JSON settings.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <returns>The JSON text.</returns>
    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, Options);
    }

    /// <summary>
    /// Deserializes JSON with the ClickUp JSON settings.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="json">The JSON text.</param>
    /// <returns>The deserialized value.</returns>
    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        options.Converters.Add(new UnixMillisecondsDateTimeOffsetConverter());
        options.Converters.Add(new UnixMillisecondsNullableDateTimeOffsetConverter());
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
