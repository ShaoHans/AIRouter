using Microsoft.Extensions.Logging;

namespace AIRouter.Core.ClientHandlers;

internal class OpenAIHttpClientHandler(ILogger<OpenAIHttpClientHandler> logger) : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var escapedString = await request.Content?.ReadAsStringAsync(cancellationToken)!;
        var content = System.Text.RegularExpressions.Regex.Unescape(escapedString);

        logger.LogInformation(
            "Sending '{Request.Method}' to '{Request.Host}{Request.Path}' with content {Request.Content}",
            request.Method,
            request.RequestUri?.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped),
            request.RequestUri!.PathAndQuery,
            content
        );

        var response = await base.SendAsync(request, cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        logger.LogInformation(
            "Received '{Response.StatusCodeInt} {Response.StatusCodeString}' with content {Response.Content}",
            (int)response.StatusCode,
            response.StatusCode,
            responseContent
        );

        // 如果使用Agent.InvokeStreamingAsync流式输出，需要将响应流重置到最开始位置
        //MemoryStream responseStream = new(await response.Content.ReadAsByteArrayAsync(cancellationToken));
        //responseStream.Position = 0;
        //response.Content = new StreamContent(responseStream);

        return response;
    }
}
