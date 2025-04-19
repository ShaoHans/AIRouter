using AIRouter.Console.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Filters;

internal class C02审计过滤器
{
    public static async Task TestAsync(Kernel kernel) 
    {
        kernel.AutoFunctionInvocationFilters.Clear();
        kernel.AutoFunctionInvocationFilters.Add(new FunctionCallsAuditFilter());
        kernel.Plugins.AddFromType<MyMathPlugin>();

        var settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
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

internal class FunctionCallsAuditFilter : IAutoFunctionInvocationFilter
{
    public async Task OnAutoFunctionInvocationAsync(
        AutoFunctionInvocationContext context,
        Func<AutoFunctionInvocationContext, Task> next
    )
    {
        var functionCalls = FunctionCallContent.GetFunctionCalls(context.ChatHistory.Last());
        if (functionCalls is not null)
        {
            foreach (var functionCall in functionCalls)
            {
                System.Console.WriteLine(
                    $"审计日志：Request {context.RequestSequenceIndex}，Function call：{functionCall.PluginName}.{functionCall.FunctionName}"
                );
            }
        }

        await next(context);
    }
}
