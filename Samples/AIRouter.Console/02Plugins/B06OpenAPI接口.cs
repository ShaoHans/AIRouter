using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace AIRouter.Console.Plugins;

internal class B06OpenAPI接口
{
    public static async Task TestAsync(Kernel kernel)
    {
        var logger = kernel
            .Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger<B06OpenAPI接口>();
        var plugin = await kernel.ImportPluginFromOpenApiAsync(
            pluginName: "WebApiPlugin",
            uri: new Uri("https://localhost:7030/openapi/v1.json") // Samples/AIRouter.WebAPI示例项目
        );
        logger.LogInformation("WebApi插件信息：{plugin}", plugin);

        var result = await plugin["GetWeatherForecast"].InvokeAsync(kernel);

        //var result = await kernel.InvokePromptAsync("获取天气预报");
        System.Console.WriteLine(result.ToString());
    }
}
