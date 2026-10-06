using System.Globalization;
using System.Text;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Infrastructure;

internal sealed class ClickUpQuery
{
    private readonly List<KeyValuePair<string, string>> _pairs = new();

    public ClickUpQuery Add(string name, string? value)
    {
        if (value is not null)
        {
            _pairs.Add(new KeyValuePair<string, string>(name, value));
        }

        return this;
    }

    public ClickUpQuery Add(string name, bool? value)
    {
        if (value.HasValue)
        {
            _pairs.Add(new KeyValuePair<string, string>(name, value.Value ? "true" : "false"));
        }

        return this;
    }

    public ClickUpQuery Add(string name, int? value)
    {
        if (value.HasValue)
        {
            _pairs.Add(new KeyValuePair<string, string>(name, value.Value.ToString(CultureInfo.InvariantCulture)));
        }

        return this;
    }

    public ClickUpQuery Add(string name, long? value)
    {
        if (value.HasValue)
        {
            _pairs.Add(new KeyValuePair<string, string>(name, value.Value.ToString(CultureInfo.InvariantCulture)));
        }

        return this;
    }

    public ClickUpQuery Add(string name, DateTimeOffset? value)
    {
        if (value.HasValue)
        {
            _pairs.Add(new KeyValuePair<string, string>(
                name,
                ClickUpTime.ToUnixMilliseconds(value.Value).ToString(CultureInfo.InvariantCulture)));
        }

        return this;
    }

    public ClickUpQuery AddMany(string name, IEnumerable<string>? values)
    {
        if (values is null)
        {
            return this;
        }

        foreach (var value in values)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _pairs.Add(new KeyValuePair<string, string>(name, value));
            }
        }

        return this;
    }

    public ClickUpQuery AddMany(string name, IEnumerable<int>? values)
    {
        if (values is null)
        {
            return this;
        }

        foreach (var value in values)
        {
            _pairs.Add(new KeyValuePair<string, string>(name, value.ToString(CultureInfo.InvariantCulture)));
        }

        return this;
    }

    public string Apply(string path)
    {
        if (_pairs.Count == 0)
        {
            return path;
        }

        var builder = new StringBuilder(path);
        builder.Append(path.IndexOf('?') >= 0 ? '&' : '?');
        for (var index = 0; index < _pairs.Count; index++)
        {
            if (index > 0)
            {
                builder.Append('&');
            }

            builder.Append(Uri.EscapeDataString(_pairs[index].Key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(_pairs[index].Value));
        }

        return builder.ToString();
    }
}
