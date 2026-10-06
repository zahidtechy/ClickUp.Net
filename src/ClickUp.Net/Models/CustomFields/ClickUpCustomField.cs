using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Net.Models;

/// <summary>
/// A custom field definition or a custom field value returned on a task.
/// </summary>
public sealed class ClickUpCustomField
{
    /// <summary>Gets or sets the field identifier.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Gets or sets the field name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the field type, such as <c>text</c>, <c>number</c>, <c>date</c>, <c>drop_down</c>, or <c>labels</c>.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the raw type configuration. The shape depends on <see cref="Type"/>.
    /// </summary>
    [JsonPropertyName("type_config")]
    public JsonElement? TypeConfig { get; set; }

    /// <summary>Gets or sets the time the field was created.</summary>
    [JsonPropertyName("date_created")]
    public DateTimeOffset? DateCreated { get; set; }

    /// <summary>Gets or sets a value indicating whether the field is hidden from guests.</summary>
    [JsonPropertyName("hide_from_guests")]
    public bool? HideFromGuests { get; set; }

    /// <summary>Gets or sets a value indicating whether the field is required.</summary>
    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    /// <summary>Gets or sets the raw field value. Use the typed helpers to read common shapes.</summary>
    [JsonPropertyName("value")]
    public JsonElement? Value { get; set; }

    /// <summary>Gets or sets rich text for fields that return it.</summary>
    [JsonPropertyName("value_richtext")]
    public string? ValueRichText { get; set; }

    /// <summary>Gets or sets Markdown for fields that return it.</summary>
    [JsonPropertyName("value_markdown")]
    public string? ValueMarkdown { get; set; }

    /// <summary>Gets or sets the objects the field applies to, when requested.</summary>
    [JsonPropertyName("applied_objects")]
    public IReadOnlyList<ClickUpCustomFieldAppliedObject>? AppliedObjects { get; set; }

    /// <summary>Reads <see cref="Value"/> when it is a string.</summary>
    /// <returns>The string value, or null when the value is missing or not a string.</returns>
    public string? GetTextValue()
    {
        return Value is { ValueKind: JsonValueKind.String } element ? element.GetString() : null;
    }

    /// <summary>Reads <see cref="Value"/> when it is a boolean.</summary>
    /// <returns>The boolean value, or null when the value is missing or not a boolean.</returns>
    public bool? GetCheckboxValue()
    {
        if (Value is not { } element)
        {
            return null;
        }

        if (element.ValueKind == JsonValueKind.True)
        {
            return true;
        }

        if (element.ValueKind == JsonValueKind.False)
        {
            return false;
        }

        return null;
    }

    /// <summary>Reads <see cref="Value"/> when it is a number.</summary>
    /// <returns>The number, or null when the value is missing or not a number.</returns>
    public decimal? GetNumberValue()
    {
        return Value is { ValueKind: JsonValueKind.Number } element && element.TryGetDecimal(out var number)
            ? number
            : null;
    }

    /// <summary>Reads <see cref="Value"/> when it is a Unix millisecond timestamp.</summary>
    /// <returns>The timestamp, or null when the value is missing or not numeric.</returns>
    public DateTimeOffset? GetDateValue()
    {
        if (Value is not { } element)
        {
            return null;
        }

        if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out var milliseconds))
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }

        if (element.ValueKind == JsonValueKind.String &&
            long.TryParse(element.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(parsed);
        }

        return null;
    }
}

/// <summary>
/// An object that a custom field applies to. Object type <c>19</c> is a custom task type.
/// </summary>
public sealed class ClickUpCustomFieldAppliedObject
{
    /// <summary>Gets or sets the applied object type.</summary>
    [JsonPropertyName("object_type")]
    public int? ObjectType { get; set; }

    /// <summary>Gets or sets the applied object identifier.</summary>
    [JsonPropertyName("object_id")]
    public long? ObjectId { get; set; }
}

/// <summary>
/// A filter entry passed to task queries in the <c>custom_fields</c> query parameter.
/// </summary>
public sealed class ClickUpCustomFieldFilter
{
    /// <summary>Gets or sets the custom field identifier.</summary>
    [JsonPropertyName("field_id")]
    public string FieldId { get; set; } = string.Empty;

    /// <summary>Gets or sets the filter operator, such as <c>=</c>, <c>!=</c>, or <c>IS NULL</c>.</summary>
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = "=";

    /// <summary>Gets or sets the raw comparison value. Omit it for operators that do not take a value.</summary>
    [JsonPropertyName("value")]
    public JsonElement? Value { get; set; }
}

/// <summary>
/// Operators documented for ClickUp custom field task filters.
/// </summary>
public static class ClickUpCustomFieldOperators
{
    /// <summary>Equal.</summary>
    public const string Equal = "=";

    /// <summary>Not equal.</summary>
    public const string NotEqual = "!=";

    /// <summary>Less than.</summary>
    public const string LessThan = "<";

    /// <summary>Less than or equal.</summary>
    public const string LessThanOrEqual = "<=";

    /// <summary>Greater than.</summary>
    public const string GreaterThan = ">";

    /// <summary>Greater than or equal.</summary>
    public const string GreaterThanOrEqual = ">=";

    /// <summary>Value is null.</summary>
    public const string IsNull = "IS NULL";

    /// <summary>Value is not null.</summary>
    public const string IsNotNull = "IS NOT NULL";

    /// <summary>Value is inside a range.</summary>
    public const string Range = "RANGE";

    /// <summary>Value matches any entry.</summary>
    public const string Any = "ANY";

    /// <summary>Value matches all entries.</summary>
    public const string All = "ALL";
}

internal sealed class ClickUpCustomFieldsResponse
{
    [JsonPropertyName("fields")]
    public IReadOnlyList<ClickUpCustomField>? Fields { get; set; }
}
