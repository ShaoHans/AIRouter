using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Plugins.OpenApi;

namespace AIRouter.Console.Plugins;

internal class B06OpenAPI接口
{
    public static async Task TestAsync(Kernel kernel)
    {
        kernel.Plugins.Clear();
        var logger = kernel
            .Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger<B06OpenAPI接口>();
        var plugin = await kernel.ImportPluginFromOpenApiAsync(
            pluginName: "WebApiPlugin",
            uri: new Uri("https://localhost:7030/openapi/v1.json") // Samples/AIRouter.WebAPI示例项目
        );
        logger.LogInformation("WebApi插件信息：{plugin}", plugin);

        var result = await plugin["GetWeatherForecast"].InvokeAsync(kernel);
        System.Console.WriteLine(result.ToString());

        result = await kernel.InvokePromptAsync(
            "总共有多少个会议室",
            new KernelArguments(
                new OpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                }
            )
        );
        System.Console.WriteLine(result.ToString());
    }

    public static async Task ReserveMeetingRoomAsync(Kernel kernel)
    {
        var logger = kernel
            .Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger<B06OpenAPI接口>();

        kernel.Plugins.Clear();
        kernel.Plugins.AddFromType<TimePlugin>();
        var plugin = await kernel.ImportPluginFromOpenApiAsync(
            pluginName: "MeetingRoomPlugin",
            uri: new Uri("https://localhost:7030/openapi/v1.json"), // Samples/AIRouter.WebAPI示例项目
            executionParameters: new OpenApiFunctionExecutionParameters 
            { 
                OperationsToExclude = ["GetWeatherForecast"]
            }
        );
        logger.LogInformation("MeetingRoomPlugin插件信息：{plugin}", plugin);

        var result = await kernel.InvokePromptAsync(
            "帮我预定今天上午10点到12点的会议室，人数50人",
            //"帮我预定今天上午10点到12点的会议室，人数5人",
            new KernelArguments(
                new OpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                }
            )
        );
        System.Console.WriteLine(result.ToString());
    }
}
