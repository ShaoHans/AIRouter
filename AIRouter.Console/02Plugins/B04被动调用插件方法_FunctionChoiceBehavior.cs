using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Plugins;

internal class B04被动调用插件方法_FunctionChoiceBehavior
{
    public static async Task TestAsync(Kernel kernel)
    {
        kernel.Plugins.AddFromType<MyMathPlugin>();

        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(autoInvoke: false)
        };

        System.Console.WriteLine("你可以让AI助理做算术运算");
        var prompt = System.Console.ReadLine();
        ChatHistory history = [];
        history.AddUserMessage(prompt!);

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        while (true)
        {
            var result = await chatCompletionService.GetChatMessageContentAsync(
                history,
                settings,
                kernel
            );
            if (result.Content is not null)
            {
                System.Console.WriteLine(result.Content.ToString());
                history.Add(result);
            }

            var functionCalls = FunctionCallContent.GetFunctionCalls(result);
            if (!functionCalls.Any())
            {
                break;
            }

            foreach (var functionCall in functionCalls)
            {
                try
                {
                    var functionResult = await functionCall.InvokeAsync(kernel);
                    history.Add(functionResult.ToChatMessage());
                }
                catch (Exception ex)
                {
                    history.Add(new FunctionResultContent(functionCall, ex).ToChatMessage());
                }
            }
        }
    }
}
