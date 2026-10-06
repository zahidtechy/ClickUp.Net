using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A custom field value written to ClickUp. Use the factory methods for the documented field shapes.
/// </summary>
public sealed class ClickUpCustomFieldValue
{
    private readonly Action<Utf8JsonWriter> _write;

    private ClickUpCustomFieldValue(Action<Utf8JsonWriter> write, bool includeTime)
    {
        _write = write;
        IncludeTime = includeTime;
    }

    /// <summary>Gets a value indicating whether a date value should include a time component.</summary>
    public bool IncludeTime { get; }

    internal void WriteJson(Utf8JsonWriter writer)
    {
        _write(writer);
    }

    /// <summary>Creates a text, URL, email, or phone value.</summary>
    /// <param name="value">The text to store. Null clears the value when the endpoint accepts null.</param>
    public static ClickUpCustomFieldValue Text(string? value)
    {
        return new ClickUpCustomFieldValue(writer =>
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value);
            }
        }, includeTime: false);
    }

    /// <summary>Creates a number, currency, or money value.</summary>
    /// <param name="value">The numeric value.</param>
    public static ClickUpCustomFieldValue Number(decimal? value)
    {
        return new ClickUpCustomFieldValue(writer =>
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteNumberValue(value.Value);
            }
        }, includeTime: false);
    }

    /// <summary>Creates a date value as Unix milliseconds.</summary>
    /// <param name="value">The date to store.</param>
    /// <param name="includeTime">Whether ClickUp should keep the time component.</param>
    public static ClickUpCustomFieldValue Date(DateTimeOffset? value, bool includeTime = false)
    {
        return new ClickUpCustomFieldValue(writer =>
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteNumberValue(value.Value.ToUnixTimeMilliseconds());
            }
        }, includeTime);
    }

    /// <summary>Creates a checkbox value.</summary>
    /// <param name="value">The checkbox state.</param>
    public static ClickUpCustomFieldValue Checkbox(bool? value)
    {
        return new ClickUpCustomFieldValue(writer =>
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteBooleanValue(value.Value);
            }
        }, includeTime: false);
    }

    /// <summary>Creates a dropdown value from an option identifier.</summary>
    /// <param name="optionId">The dropdown option identifier.</param>
    public static ClickUpCustomFieldValue Dropdown(string? optionId)
    {
        return Text(optionId);
    }

    /// <summary>Creates a dropdown value from a numeric option identifier.</summary>
    /// <param name="optionId">The dropdown option identifier.</param>
    public static ClickUpCustomFieldValue Dropdown(int optionId)
    {
        return new ClickUpCustomFieldValue(writer => writer.WriteNumberValue(optionId), includeTime: false);
    }

    /// <summary>Creates a labels value that adds and removes label identifiers.</summary>
    /// <param name="add">Label identifiers to add.</param>
    /// <param name="remove">Label identifiers to remove.</param>
    public static ClickUpCustomFieldValue Labels(IEnumerable<string>? add, IEnumerable<string>? remove)
    {
        return AddRemove(add, remove, static (writer, item) => writer.WriteStringValue(item));
    }

    /// <summary>Creates a users value that adds and removes user identifiers.</summary>
    /// <param name="add">User identifiers to add.</param>
    /// <param name="remove">User identifiers to remove.</param>
    public static ClickUpCustomFieldValue Users(IEnumerable<int>? add, IEnumerable<int>? remove)
    {
        return AddRemove(add, remove, static (writer, item) => writer.WriteNumberValue(item));
    }

    /// <summary>Creates a tasks relationship value.</summary>
    /// <param name="taskIds">The related task identifiers.</param>
    public static ClickUpCustomFieldValue Tasks(IEnumerable<string>? taskIds)
    {
        return new ClickUpCustomFieldValue(writer =>
        {
            writer.WriteStartArray();
            if (taskIds is not null)
            {
                foreach (var taskId in taskIds)
                {
                    writer.WriteStringValue(taskId);
                }
            }

            writer.WriteEndArray();
        }, includeTime: false);
    }

    /// <summary>Creates a manual progress value.</summary>
    /// <param name="current">The current progress value.</param>
    public static ClickUpCustomFieldValue ManualProgress(decimal current)
    {
        return new ClickUpCustomFieldValue(writer =>
        {
            writer.WriteStartObject();
            writer.WriteNumber("current", current);
            writer.WriteEndObject();
        }, includeTime: false);
    }

    /// <summary>Creates a location value.</summary>
    /// <param name="latitude">The latitude.</param>
    /// <param name="longitude">The longitude.</param>
    /// <param name="formattedAddress">The formatted address, when available.</param>
    public static ClickUpCustomFieldValue Location(double latitude, double longitude, string? formattedAddress)
    {
        return new ClickUpCustomFieldValue(writer =>
        {
            writer.WriteStartObject();
            writer.WritePropertyName("location");
            writer.WriteStartObject();
            writer.WriteNumber("lat", latitude);
            writer.WriteNumber("lng", longitude);
            writer.WriteEndObject();
            if (formattedAddress is not null)
            {
                writer.WriteString("formatted_address", formattedAddress);
            }

            writer.WriteEndObject();
        }, includeTime: false);
    }

    /// <summary>Creates a value from raw JSON for a field shape that does not have a dedicated helper.</summary>
    /// <param name="value">The raw JSON value.</param>
    public static ClickUpCustomFieldValue Raw(JsonElement value)
    {
        return new ClickUpCustomFieldValue(value.WriteTo, includeTime: false);
    }

    private static ClickUpCustomFieldValue AddRemove<T>(
        IEnumerable<T>? add,
        IEnumerable<T>? remove,
        Action<Utf8JsonWriter, T> writeItem)
    {
        return new ClickUpCustomFieldValue(writer =>
        {
            writer.WriteStartObject();
            writer.WritePropertyName("add");
            WriteArray(writer, add, writeItem);
            writer.WritePropertyName("rem");
            WriteArray(writer, remove, writeItem);
            writer.WriteEndObject();
        }, includeTime: false);
    }

    private static void WriteArray<T>(Utf8JsonWriter writer, IEnumerable<T>? values, Action<Utf8JsonWriter, T> writeItem)
    {
        writer.WriteStartArray();
        if (values is not null)
        {
            foreach (var value in values)
            {
                writeItem(writer, value);
            }
        }

        writer.WriteEndArray();
    }
}

/// <summary>
/// A custom field value included when creating a task.
/// </summary>
[JsonConverter(typeof(ClickUpTaskCustomFieldConverter))]
public sealed class ClickUpTaskCustomField
{
    /// <summary>Gets or sets the custom field identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the value to store.</summary>
    public ClickUpCustomFieldValue Value { get; set; } = ClickUpCustomFieldValue.Text(null);
}

/// <summary>
/// Request body for setting a task custom field value.
/// </summary>
[JsonConverter(typeof(SetCustomFieldValueRequestConverter))]
public sealed class SetCustomFieldValueRequest
{
    /// <summary>Gets or sets the value to store.</summary>
    public ClickUpCustomFieldValue Value { get; set; } = ClickUpCustomFieldValue.Text(null);
}

internal sealed class ClickUpTaskCustomFieldConverter : JsonConverter<ClickUpTaskCustomField>
{
    public override ClickUpTaskCustomField Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("ClickUp task custom field assignments are write-only.");
    }

    public override void Write(Utf8JsonWriter writer, ClickUpTaskCustomField value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("id", value.Id);
        writer.WritePropertyName("value");
        value.Value.WriteJson(writer);
        if (value.Value.IncludeTime)
        {
            writer.WritePropertyName("value_options");
            writer.WriteStartObject();
            writer.WriteBoolean("time", true);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
}

internal sealed class SetCustomFieldValueRequestConverter : JsonConverter<SetCustomFieldValueRequest>
{
    public override SetCustomFieldValueRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("ClickUp custom field value requests are write-only.");
    }

    public override void Write(Utf8JsonWriter writer, SetCustomFieldValueRequest value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("value");
        value.Value.WriteJson(writer);
        if (value.Value.IncludeTime)
        {
            writer.WritePropertyName("value_options");
            writer.WriteStartObject();
            writer.WriteBoolean("time", true);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
}
