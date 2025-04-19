using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AIRouter.Console.Plugins;

internal class B02手动调用插件方法_ToolCallBehavior
{
    public static async Task TestAsync(Kernel kernel)
    {
        kernel.Plugins.AddFromType<MyMathPlugin>();
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings =
            new() { ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions };

        var chatHistory = new ChatHistory();
        System.Console.WriteLine("请输入加法的两个参数：");
        var input = System.Console.ReadLine();
        chatHistory.AddUserMessage(input!);
        var chatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        var result = await chatCompletion.GetChatMessageContentAsync(
            chatHistory,
            openAIPromptExecutionSettings,
            kernel
        );

        // 获取LLMS返回的函数调用，手动调用这些插件方法
        var functionCalls = FunctionCallContent.GetFunctionCalls(result);
        foreach (var fc in functionCalls)
        {
            var content = await fc.InvokeAsync(kernel);
            // 将结果加到聊天记录中
            chatHistory.Add(content.ToChatMessage());
        }

        #region 另外一种手动调用的写法，有异常

        //// 获取LLMs返回的函数调用
        //var functionCalls = ((OpenAIChatMessageContent)result).GetOpenAIFunctionToolCalls();

        //foreach (var functionCall in functionCalls)
        //{
        //    // 调用函数
        //    var functionResult = await kernel.InvokeAsync(
        //        functionCall.PluginName,
        //        functionCall.FunctionName,
        //        new KernelArguments(functionCall.Arguments)
        //    );

        //    var jsonResponse = functionResult.GetValue<object>();
        //    var json = JsonSerializer.Serialize(jsonResponse);

        //    // 添加到聊天历史，注意角色是Tool
        //    chatHistory.AddMessage(
        //        AuthorRole.Tool,
        //        json,
        //        metadata: new Dictionary<string, object?> { ["tool_call_id"] = functionCall.Id }
        //    );
        //}
        #endregion

        // 再次调用LLMs将计算结果返回给LLMs，以便获得最终输出结果
        var finnalResult = await chatCompletion.GetChatMessageContentAsync(
            chatHistory: chatHistory,
            kernel: kernel
        );
        System.Console.WriteLine(finnalResult.ToString());
    }
}
