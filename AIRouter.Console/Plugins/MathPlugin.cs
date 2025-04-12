using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AIRouter.Console.Plugins;

internal class MathPlugin
{
    [KernelFunction, Description("两个数相加")]
    public static int Add([Description("第一个数")] int a, [Description("第二个数")] int b)
    {
        return a + b;
    }
}
