namespace AIRouter.Core.Ollama;

internal class OllamaHttpClientHandler(string url) : HttpClientHandler
{
    private readonly string _url = url.TrimEnd('/');

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        if (
            request.RequestUri != null
            && request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase)
        )
        {
            request.RequestUri = new Uri($"{_url}{request.RequestUri.PathAndQuery}");
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
