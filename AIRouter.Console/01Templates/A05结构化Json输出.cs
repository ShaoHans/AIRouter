using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Templates;

internal class A05结构化Json输出
{
    public static async Task TestAsync(Kernel kernel)
    {
        var settings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(MilitaryOfficer),
        };

        var result = await kernel.InvokePromptAsync(
            "三国时期武力值排行前10的人？以json格式输出，包含姓名、出生年月、武力值、简介等",
            new(settings)
        );
        System.Console.WriteLine(result);
    }
}

internal class MilitaryOfficer
{
    [Description("姓名")]
    public string Name { get; set; }

    [Description("出生年月")]
    public string BirthDate { get; set; }

    [Description("武力值(满分100)")]
    public int ForceValue { get; set; }

    [Description("简介")]
    public string Description { get; set; }
}
