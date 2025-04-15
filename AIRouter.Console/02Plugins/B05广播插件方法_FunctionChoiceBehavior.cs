using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Plugins;

internal class B05广播插件方法_FunctionChoiceBehavior
{
    public static async Task TestRequiredAsync(Kernel kernel)
    {
        kernel.Plugins.AddFromType<MyMathPlugin>();
        var settings = new OpenAIPromptExecutionSettings
        {
            // 禁用Add函数
            FunctionChoiceBehavior = FunctionChoiceBehavior.Required(
                [kernel.Plugins.GetFunction("MyMathPlugin", "Add")]
            )
        };

        System.Console.WriteLine("你可以让AI助理做算术运算");
        // 如果让AI做加法运算会报错
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

    public static async Task TestNoneAsync(Kernel kernel)
    {
        kernel.Plugins.AddFromType<MyMathPlugin>();
        var settings = new OpenAIPromptExecutionSettings
        {
            // 即仅向AI 广播函数，但不允许AI 使用函数。
            FunctionChoiceBehavior = FunctionChoiceBehavior.None()
        };

        System.Console.WriteLine(
            await kernel.InvokePromptAsync(
                "我要做加法和除法运算，应该使用哪些函数？",
                new(settings)
            )
        );
    }
}
