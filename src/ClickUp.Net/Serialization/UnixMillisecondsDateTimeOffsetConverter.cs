using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Net.Serialization;

internal sealed class UnixMillisecondsDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = UnixMillisecondsNullableDateTimeOffsetConverter.ReadValue(ref reader);
        if (value is null)
        {
            throw new JsonException("Expected a ClickUp Unix millisecond timestamp.");
        }

        return value.Value;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
    }
}

internal sealed class UnixMillisecondsNullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ReadValue(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteNumberValue(value.Value.ToUnixTimeMilliseconds());
    }

    internal static DateTimeOffset? ReadValue(ref Utf8JsonReader reader)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number when reader.TryGetInt64(out var milliseconds):
                return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
            case JsonTokenType.String:
                var text = reader.GetString();
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                {
                    return DateTimeOffset.FromUnixTimeMilliseconds(parsed);
                }

                if (DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var date))
                {
                    return date;
                }

                throw new JsonException("Unable to read a ClickUp timestamp.");
            default:
                throw new JsonException("Unable to read a ClickUp timestamp.");
        }
    }
}
