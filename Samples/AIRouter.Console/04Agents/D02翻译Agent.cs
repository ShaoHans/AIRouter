using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace AIRouter.Console.Agents;

internal class D02翻译Agent
{
    public static async Task TestAsync(Kernel kernel)
    {
        var templateConfig = new PromptTemplateConfig()
        {
            Template = "翻译成{{$language}}",
            TemplateFormat = "semantic-kernel"
        };

        var agent = new ChatCompletionAgent(templateConfig, new KernelPromptTemplateFactory())
        {
            Name = "Translate Agent",
            Instructions = "你是一位翻译助手",
            Kernel = kernel,
            Arguments = new KernelArguments { { "language", "英文" } }
        };

        ChatHistoryAgentThread thread = new();

        while (true)
        {
            System.Console.Write("User > ");
            var userMessage = System.Console.ReadLine()!;
            if (userMessage.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            System.Console.Write("Assistant > ");

            await foreach (
                AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(
                    userMessage,
                    thread
                )
            )
            {
                System.Console.Write(response.Message.Content);
                thread = (ChatHistoryAgentThread)response.Thread;
            }

            System.Console.WriteLine();
        }
    }
}
