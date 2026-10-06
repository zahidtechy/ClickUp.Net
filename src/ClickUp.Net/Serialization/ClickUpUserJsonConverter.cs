using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClickUp.Net.Models;

namespace ClickUp.Net.Serialization;

/// <summary>
/// Reads a ClickUp user object, null, or a scalar identifier without failing the parent payload.
/// </summary>
public sealed class ClickUpUserJsonConverter : JsonConverter<ClickUpUser?>
{
    /// <inheritdoc />
    public override ClickUpUser? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var numericId))
        {
            return new ClickUpUser { Id = numericId };
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var text = reader.GetString();
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                return new ClickUpUser { Id = id };
            }

            return new ClickUpUser { Username = text };
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected a ClickUp user object.");
        }

        var inner = new JsonSerializerOptions(options);
        for (var index = inner.Converters.Count - 1; index >= 0; index--)
        {
            if (inner.Converters[index] is ClickUpUserJsonConverter)
            {
                inner.Converters.RemoveAt(index);
            }
        }

        return JsonSerializer.Deserialize<ClickUpUser>(ref reader, inner);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, ClickUpUser? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var inner = new JsonSerializerOptions(options);
        for (var index = inner.Converters.Count - 1; index >= 0; index--)
        {
            if (inner.Converters[index] is ClickUpUserJsonConverter)
            {
                inner.Converters.RemoveAt(index);
            }
        }

        JsonSerializer.Serialize(writer, value, inner);
    }
}
