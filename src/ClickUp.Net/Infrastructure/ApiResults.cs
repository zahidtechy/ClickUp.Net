using System.Net;
using System.Text.Json;
using ClickUp.Net.Exceptions;
using ClickUp.Net.Models;
using ClickUp.Net.Serialization;

namespace ClickUp.Net.Infrastructure;

internal static class ApiResults
{
    public static T Required<T>(T? value, string resource) where T : class
    {
        if (value is null)
        {
            throw new ClickUpApiException(
                $"ClickUp returned an empty {resource} response.",
                HttpStatusCode.OK,
                errorCode: null,
                responseBody: null,
                requestId: null,
                rateLimit: null);
        }

        return value;
    }

    public static IReadOnlyList<T> List<T>(IReadOnlyList<T>? values)
    {
        return values ?? Array.Empty<T>();
    }

    public static string? SerializeCustomFieldFilters(IReadOnlyList<ClickUpCustomFieldFilter>? filters)
    {
        if (filters is null || filters.Count == 0)
        {
            return null;
        }

        return JsonSerializer.Serialize(filters, ClickUpSerializer.Options);
    }

    public static void ApplyTaskReference(ClickUpQuery query, ClickUpTaskReferenceOptions? options)
    {
        if (options is null)
        {
            return;
        }

        if (options.CustomTaskIds == true && string.IsNullOrWhiteSpace(options.TeamId))
        {
            throw new ArgumentException("TeamId is required when CustomTaskIds is true.", nameof(options));
        }

        query.Add("custom_task_ids", options.CustomTaskIds);
        query.Add("team_id", options.TeamId);
    }
}
