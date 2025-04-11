using Microsoft.SemanticKernel;

namespace AIRouter.Console.Templates;

internal class A02文件模板提示词
{
    public static async Task TestAsync(Kernel kernel)
    {
        System.Console.WriteLine("请输入要翻译的中文内容：");

        var plugins = kernel.CreatePluginFromPromptDirectory("Templates/Prompts");
        var input = System.Console.ReadLine();
        var result = await kernel.InvokeAsync(
            plugins["Translate"],
            new KernelArguments() { ["content"] = input }
        );
        System.Console.WriteLine(result);
    }
}
