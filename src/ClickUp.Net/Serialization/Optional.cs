namespace ClickUp.Net.Serialization;

/// <summary>
/// Distinguishes a value that was omitted from a value that was explicitly supplied, including null.
/// </summary>
/// <typeparam name="T">The underlying value type.</typeparam>
public readonly struct Optional<T>
{
    /// <summary>
    /// Initializes a specified value, including an explicit null.
    /// </summary>
    /// <param name="value">The value to send.</param>
    public Optional(T? value)
    {
        IsSpecified = true;
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether the caller set this property.
    /// </summary>
    public bool IsSpecified { get; }

    /// <summary>
    /// Gets the supplied value when <see cref="IsSpecified"/> is true.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets an optional value that will be omitted from the JSON payload.
    /// </summary>
    public static Optional<T> Unspecified => default;

    /// <summary>
    /// Creates a specified optional from the given value.
    /// </summary>
    /// <param name="value">The value to send. Null is written as JSON null.</param>
    public static implicit operator Optional<T>(T? value)
    {
        return new Optional<T>(value);
    }
}
