using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClickUp.Net.Serialization;

internal sealed class OptionalPayloadConverter<T> : JsonConverter<T> where T : class, new()
{
    private static readonly PropertyMetadata[] Properties = BuildMetadata();

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        using var document = JsonDocument.ParseValue(ref reader);
        var instance = new T();
        foreach (var property in Properties)
        {
            if (!document.RootElement.TryGetProperty(property.JsonName, out var jsonValue))
            {
                continue;
            }

            object? value = jsonValue.ValueKind == JsonValueKind.Null
                ? null
                : JsonSerializer.Deserialize(jsonValue.GetRawText(), property.InnerType, options);

            var optional = property.Constructor.Invoke(new[] { value });
            property.Property.SetValue(instance, optional);
        }

        return instance;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var property in Properties)
        {
            var optional = property.Property.GetValue(value);
            if (optional is null || !(bool)property.IsSpecified.GetValue(optional)!)
            {
                continue;
            }

            var inner = property.ValueProperty.GetValue(optional);
            writer.WritePropertyName(property.JsonName);
            JsonSerializer.Serialize(writer, inner, property.InnerType, options);
        }

        writer.WriteEndObject();
    }

    private static PropertyMetadata[] BuildMetadata()
    {
        var result = new List<PropertyMetadata>();
        foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var propertyType = property.PropertyType;
            if (!propertyType.IsGenericType || propertyType.GetGenericTypeDefinition() != typeof(Optional<>))
            {
                continue;
            }

            var innerType = propertyType.GetGenericArguments()[0];
            var constructor = propertyType.GetConstructor(new[] { innerType.IsValueType ? innerType : innerType })
                ?? propertyType.GetConstructors().FirstOrDefault(candidate => candidate.GetParameters().Length == 1);
            if (constructor is null)
            {
                throw new InvalidOperationException($"Optional property '{property.Name}' does not have a value constructor.");
            }

            var jsonName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
            result.Add(new PropertyMetadata(
                property,
                jsonName,
                innerType,
                constructor,
                propertyType.GetProperty(nameof(Optional<int>.IsSpecified))!,
                propertyType.GetProperty(nameof(Optional<int>.Value))!));
        }

        return result.ToArray();
    }

    private sealed class PropertyMetadata
    {
        public PropertyMetadata(
            PropertyInfo property,
            string jsonName,
            Type innerType,
            ConstructorInfo constructor,
            PropertyInfo isSpecified,
            PropertyInfo valueProperty)
        {
            Property = property;
            JsonName = jsonName;
            InnerType = innerType;
            Constructor = constructor;
            IsSpecified = isSpecified;
            ValueProperty = valueProperty;
        }

        public PropertyInfo Property { get; }

        public string JsonName { get; }

        public Type InnerType { get; }

        public ConstructorInfo Constructor { get; }

        public PropertyInfo IsSpecified { get; }

        public PropertyInfo ValueProperty { get; }
    }
}
