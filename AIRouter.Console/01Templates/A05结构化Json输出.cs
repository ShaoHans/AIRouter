using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Templates;

internal class A05结构化Json输出
{
    public static async Task TestAsync(Kernel kernel)
    {
        var settings = new OpenAIPromptExecutionSettings { ResponseFormat = "json_object", };

        var result = await kernel.InvokePromptAsync(
            "三国时期武力值排行前10的人？以json格式输出，包含姓名、武力值、朝代、简介",
            new(settings)
        );
        System.Console.WriteLine(result);
    }
}
