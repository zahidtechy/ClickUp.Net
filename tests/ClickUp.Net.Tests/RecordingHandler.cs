using System.Net;

namespace ClickUp.Net.Tests;

internal sealed class RecordedRequest
{
    public RecordedRequest(HttpMethod method, string uri, string? authorization, string? body)
    {
        Method = method;
        Uri = uri;
        Authorization = authorization;
        Body = body;
    }

    public HttpMethod Method { get; }

    public string Uri { get; }

    public string? Authorization { get; }

    public string? Body { get; }
}

internal sealed class RecordingHandler : HttpMessageHandler
{
    public List<RecordedRequest> Requests { get; } = new();

    public Func<RecordedRequest, CancellationToken, HttpResponseMessage>? Responder { get; set; }

    public Func<CancellationToken, Task>? BeforeResponse { get; set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        string? authorization = null;
        if (request.Headers.TryGetValues("Authorization", out var values))
        {
            authorization = string.Join(",", values);
        }

        var recorded = new RecordedRequest(request.Method, request.RequestUri?.ToString() ?? string.Empty, authorization, body);
        Requests.Add(recorded);
        if (BeforeResponse is not null)
        {
            await BeforeResponse(cancellationToken).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (Responder is null)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
            };
        }

        return Responder(recorded, cancellationToken);
    }
}
