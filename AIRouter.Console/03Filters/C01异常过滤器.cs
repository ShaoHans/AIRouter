using AIRouter.Console.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Filters;

internal class C01异常过滤器
{
    public static async Task TestAsync(Kernel kernel)
    {
        kernel.FunctionInvocationFilters.Clear();
        kernel.FunctionInvocationFilters.Add(new ExceptionHandleFilter());
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

internal class ExceptionHandleFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, Task> next
    )
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Result = new FunctionResult(context.Result, ex);
        }
    }
}
