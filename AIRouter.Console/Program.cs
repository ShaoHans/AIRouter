using AIRouter.Console.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

//var kernel = Kernel.CreateBuilder().AddChatCompletion(configuration, "ollama").Build();

var services = new ServiceCollection();
services.RegisterKernels(configuration);
services.AddSerilog(configuration);
var sp = services.BuildServiceProvider();
var kernel = sp.GetRequiredKeyedService<Kernel>("zhipu");

#region 提示词

//await A01内联提示词.TestAsync(kernel);
//await A02文件模板提示词.TestTxtAsync(kernel);
//await A02文件模板提示词.TestYamlAsync(kernel);
//await A03Handlebars模板提示词.TestAsync(kernel);
//await A04Liquid模板提示词.TestAsync(kernel);

#endregion

//await B01自动调用插件方法_ToolCallBehavior.AutoInvokePluginMethodAsync(kernel);
//await B01自动调用插件方法_ToolCallBehavior.AutoInvokeKernelFunctionsAsync(kernel);
//await B02手动调用插件方法_ToolCallBehavior.TestAsync(kernel);
//await B03内置插件.TestAsync(kernel);
//await B04自动调用插件方法_FunctionChoiceBehavior.TestAsync(kernel);
//await B04被动调用插件方法_FunctionChoiceBehavior.TestAsync(kernel);
//await B05广播插件方法_FunctionChoiceBehavior.TestRequiredAsync(kernel);
await B05广播插件方法_FunctionChoiceBehavior.TestNoneAsync(kernel);

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
