using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AIRouter.Console.Plugins;

internal class MyMathPlugin
{
    [KernelFunction, Description("两个数相加")]
    public static int Add([Description("第一个数")] int a, [Description("第二个数")] int b)
    {
        return a + b;
    }

    [KernelFunction, Description("两个数相减")]
    public static int Subtract([Description("第一个数")] int a, [Description("第二个数")] int b)
    {
        return a - b;
    }

    [KernelFunction, Description("两个数相乘")]
    public static int Multiply([Description("第一个数")] int a, [Description("第二个数")] int b)
    {
        return a * b;
    }

    [KernelFunction, Description("两个数相除")]
    public static double Divide([Description("第一个数")] int a, [Description("第二个数")] int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("除数不能为零");
        }
        return (double)a / b;
    }

    [KernelFunction, Description("取模")]
    public static int Mod([Description("第一个数")] int a, [Description("第二个数")] int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("炸锅了！！！");
        }
        return a % b;
    }
}
