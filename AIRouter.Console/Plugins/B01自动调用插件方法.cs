using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Plugins;

internal class B01自动调用插件方法
{
    public static async Task AutoInvokePluginMethodAsync(Kernel kernel)
    {
        kernel.Plugins.AddFromType<MyMathPlugin>();
        var anwser = await kernel.InvokeAsync<int>(
            "MathPlugin",
            "Add",
            new KernelArguments() { ["a"] = 1, ["b"] = 2 }
        );
        System.Console.WriteLine(anwser);

        kernel.Plugins.AddFromFunctions(
            "TimePlugin",
            [
                KernelFunctionFactory.CreateFromMethod(
                    method: () => DateTime.Now,
                    functionName: "GetCurrentTime",
                    description: "获取当前系统时间"
                )
            ]
        );
        var time = await kernel.InvokeAsync<DateTime>("TimePlugin", "GetCurrentTime");
        System.Console.WriteLine(time);
    }

    public static async Task AutoInvokeKernelFunctionsAsync(Kernel kernel)
    {
        kernel.Plugins.Clear();
        kernel.Plugins.AddFromType<MyMathPlugin>();

        var openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        System.Console.WriteLine("请输入加法的两个参数：");
        var input = System.Console.ReadLine();
        var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        var result = await chatCompletion.GetChatMessageContentAsync(
            input!,
            openAIPromptExecutionSettings,
            kernel
        );
        System.Console.WriteLine(result.ToString());
    }
}
