using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Net.Serialization;

/// <summary>
/// Reads a JSON string or number into a string. ClickUp uses both shapes for some identifiers and indexes.
/// </summary>
public sealed class StringOrNumberJsonConverter : JsonConverter<string?>
{
    /// <inheritdoc />
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Number when reader.TryGetInt64(out var integer):
                return integer.ToString(CultureInfo.InvariantCulture);
            case JsonTokenType.Number:
                return reader.GetDouble().ToString(CultureInfo.InvariantCulture);
            default:
                throw new JsonException("Expected a JSON string or number.");
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value);
    }
}
