using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace AIRouter.Console.Agents;

internal class D01简单问答Agent
{
    public static async Task TestAsync(Kernel kernel)
    {
        var agent = new ChatCompletionAgent
        {
            Name = "AI问答助手",
            Instructions = "你是一个问答助手，回答用户的问题。",
            Kernel = kernel
        };

        // Define a thread variable to maintain the conversation context.
        // Since we are passing a null thread to InvokeAsync on the first invocation,
        // the agent will create a new thread for the conversation.
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

            //var asyncResults = agent.InvokeStreamingAsync(userMessage, thread);
            //await foreach(var a in asyncResults)
            //{
            //    System.Console.Write(a.Message.Content);
            //}

            //await foreach (var response in agent.InvokeStreamingAsync(userMessage, thread))
            //{
            //    System.Console.Write($"{response.Message.Content}"); // Stream to console
            //}

            await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(userMessage, thread))
            {
                System.Console.Write(response.Message.Content);
                thread = (ChatHistoryAgentThread)response.Thread;
            }

            System.Console.WriteLine();
        }
    }
}
