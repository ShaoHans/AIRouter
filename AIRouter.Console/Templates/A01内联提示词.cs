using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Templates;

internal class A01内联提示词
{
    public static async Task TestAsync(Kernel kernel)
    {
        var promt = "将以下中文翻译成英文：{{$content}}";

        var function = kernel.CreateFunctionFromPrompt(promt, new OpenAIPromptExecutionSettings());
        var content = "好好学习，天天向上";
        var result = await kernel.InvokeAsync(function, new KernelArguments() { ["content"] = content });
        System.Console.WriteLine(result);
    }
}
