namespace z018.EodHistoricalDataTests.Helper;

using System.Net;

internal class MockHttpResponseMessage : HttpMessageHandler
{
    private readonly Dictionary<string, HttpResponseMessage> messages;

    public MockHttpResponseMessage(Dictionary<string, HttpResponseMessage> messages)
    {
        this.messages = messages;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri == null) throw new NullReferenceException(nameof(request.RequestUri));

        string uri = request.RequestUri.ToString();
        var response = messages.TryGetValue(uri, out var value)
            ? value ?? new HttpResponseMessage(HttpStatusCode.NoContent)
            : new HttpResponseMessage(HttpStatusCode.NotFound);

        response.RequestMessage = request;
        return Task.FromResult(response);
    }
}