using AIRouter.Console.Filters;
using AIRouter.Console.Plugins;
using AIRouter.Console.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>();
    })
    .ConfigureServices(
        (context, services) =>
        {
            services.RegisterKernels(context.Configuration);
            services.AddSerilog();
        }
    )
    .Build();

var kernel = host.Services.GetRequiredKeyedService<Kernel>("zhipu");

#region 01Templates

//await A01内联提示词.TestAsync(kernel);
//await A02文件模板提示词.TestTxtAsync(kernel);
//await A02文件模板提示词.TestYamlAsync(kernel);
//await A03Handlebars模板提示词.TestAsync(kernel);
//await A04Liquid模板提示词.TestAsync(kernel);
//await A05结构化Json输出.TestAsync(kernel);

#endregion

#region 02Plugins
//await B01自动调用插件方法_ToolCallBehavior.AutoInvokePluginMethodAsync(kernel);
//await B01自动调用插件方法_ToolCallBehavior.AutoInvokeKernelFunctionsAsync(kernel);
//await B02手动调用插件方法_ToolCallBehavior.TestAsync(kernel);
//await B03内置插件.TestAsync(kernel);
//await B04自动调用插件方法_FunctionChoiceBehavior.TestAsync(kernel);
//await B04被动调用插件方法_FunctionChoiceBehavior.TestAsync(kernel);
//await B05广播插件方法_FunctionChoiceBehavior.TestRequiredAsync(kernel);
//await B05广播插件方法_FunctionChoiceBehavior.TestNoneAsync(kernel);
//await B06OpenAPI接口.TestAsync(kernel);
await B06OpenAPI接口.ReserveMeetingRoomAsync(kernel);

#endregion

#region 03Filters

//await C01异常过滤器.TestAsync(kernel);
//await C02审计过滤器.TestAsync(kernel);

#endregion

await host.RunAsync();

return;
var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() { };
var chatMessages = new ChatHistory(
    """
    你是一个友善且乐于遵守规则的助手。在采取任何重要行动前，你都会按要求完成必要步骤并请求确认。
    如果用户未提供足够信息，你将持续提问直至获得完成任务所需的全部内容。
    """
);
while (true)
{
    Console.Write("User > ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
        chatMessages,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel
    );

    string fullMessage = "";
    Console.Write("Assistant > ");

    await foreach (var content in result)
    {
        Console.Write(content.Content);
        fullMessage += content.Content;
    }
    Console.WriteLine();

    chatMessages.AddAssistantMessage(fullMessage);
}
