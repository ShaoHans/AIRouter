using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using ModelContextProtocol.Client;
using OpenAI.Chat;

namespace AIRouter.Console.MCP;

internal class F01Github
{
    public static async Task<IMcpClient> GetMcpClientAsync()
    {
        var clientTransport = new StdioClientTransport(
            new StdioClientTransportOptions
            {
                Name = "GitHub",
                Command = "npx",
                Arguments = ["-y", "@modelcontextprotocol/server-github"],
            }
        );

        return await McpClientFactory.CreateAsync(clientTransport);
    }

    public static async Task ListToolsAsync()
    {
        var client = await GetMcpClientAsync();
        var tools = await client.ListToolsAsync();
        foreach (var tool in tools)
        {
            System.Console.WriteLine($"Tool: {tool.Name} - {tool.Description}");
        }
    }

    public static async Task CallToolAsync()
    {
        var client = await GetMcpClientAsync();
        var result = await client.CallToolAsync(
            toolName: "search_repositories",
            arguments: new Dictionary<string, Object?>() { ["query"] = "ShaoHans/Abp.RadzenUI" }
        );
        System.Console.WriteLine(
            $"Tool call result:{result.IsError}, {JsonSerializer.Serialize(result.Content, options: new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            })}"
        );
    }

    public static async Task SummarizeAsync(Kernel kernel)
    {
#pragma warning disable SKEXP0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。

        var chatClient = kernel
            .GetRequiredService<IChatCompletionService>()
            .AsChatClient()
            .AsBuilder()
            .UseFunctionInvocation()
            .Build();

        var tools = await (await GetMcpClientAsync()).ListToolsAsync();
        var chatOptions = new ChatOptions { Tools = [.. tools] };

        List<Microsoft.Extensions.AI.ChatMessage> messages = [];
        while (true)
        {
            System.Console.Write("Q: ");
            messages.Add(new(ChatRole.User, System.Console.ReadLine()));

            List<ChatResponseUpdate> updates = [];
            await foreach (
                var update in chatClient.GetStreamingResponseAsync(messages, chatOptions)
            )
            {
                System.Console.Write(update);
                updates.Add(update);
            }
            System.Console.WriteLine();

            messages.AddMessages(updates);
        }

#pragma warning restore SKEXP0001 // 类型仅用于评估，在将来的更新中可能会被更改或删除。取消此诊断以继续。
    }
}
