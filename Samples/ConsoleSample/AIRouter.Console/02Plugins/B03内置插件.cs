using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;

namespace AIRouter.Console.Plugins;

internal class B03内置插件
{
    public static async Task TestAsync(Kernel kernel)
    {
        kernel.Plugins.AddFromType<TimePlugin>();
        //kernel.Plugins.AddFromType<MathPlugin>();
        kernel.Plugins.AddFromType<HttpPlugin>();
        kernel.Plugins.AddFromType<ConversationSummaryPlugin>();
        kernel.Plugins.AddFromType<FileIOPlugin>();
        kernel.Plugins.AddFromType<TextPlugin>();
        //kernel.Plugins.AddFromType<WaitPlugin>();

        var settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        System.Console.WriteLine("你可以让AI助理做算术运算、获取当前系统时间等");
        var prompt = System.Console.ReadLine();
        var result = await kernel
            .GetRequiredService<IChatCompletionService>()
            .GetChatMessageContentAsync(
                prompt: prompt!,
                executionSettings: settings,
                kernel: kernel
            );
        System.Console.WriteLine(result.ToString());

    }
}
