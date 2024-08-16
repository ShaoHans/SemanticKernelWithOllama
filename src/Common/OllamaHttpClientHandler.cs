
namespace Common;

public sealed class OllamaHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri != null && request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
        {
            request.RequestUri = new Uri($"http://localhost:11434{request.RequestUri.PathAndQuery}");
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
