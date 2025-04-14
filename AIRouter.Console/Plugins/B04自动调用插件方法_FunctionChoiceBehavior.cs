using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Plugins;

internal class B04自动调用插件方法_FunctionChoiceBehavior
{
    public static async Task TestAsync(Kernel kernel)
    {
        kernel.Plugins.AddFromType<MyMathPlugin>();
        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        System.Console.WriteLine("你可以让AI助理做算术运算");
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
