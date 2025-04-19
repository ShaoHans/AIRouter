using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Filters;

internal class C01异常过滤器
{
    public static async Task TestAsync(Kernel kernel)
    {
        kernel.FunctionInvocationFilters.Clear();
        kernel.FunctionInvocationFilters.Add(
            new ExceptionHandleFilter(
                kernel.Services.GetRequiredService<ILoggerFactory>().CreateLogger<C01异常过滤器>()
            )
        );
        kernel.Plugins.AddFromFunctions(
            "TimePlugin",
            [
                KernelFunctionFactory.CreateFromMethod(
                    method: () =>
                    {
                        if (DateTime.Now.Hour == 21)
                        {
                            throw new Exception("测试FunctionInvocationFilter异常过滤器");
                        }
                        return DateTime.Now;
                    },
                    functionName: "GetCurrentTime",
                    description: "获取当前系统时间"
                )
            ]
        );
        var settings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        System.Console.WriteLine("你可以询问AI助理当前时间");
        var prompt = System.Console.ReadLine();
        var result = await kernel
            .GetRequiredService<IChatCompletionService>()
            .GetChatMessageContentAsync(
                prompt: prompt!,
                executionSettings: settings,
                kernel: kernel
            );

        System.Console.WriteLine(result.ToString());

        var function = KernelFunctionFactory.CreateFromMethod(() =>
        {
            throw new KernelException("Exception in function");
        });

        var result2 = await kernel.InvokeAsync(function);

        System.Console.WriteLine(result2);
    }
}

internal class ExceptionHandleFilter(ILogger logger) : IFunctionInvocationFilter
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
            logger.LogError(ex, "执行插件方法出现异常");
            context.Result = new FunctionResult(
                context.Result,
                "Friendly message instead of exception"
            );
        }
    }
}
