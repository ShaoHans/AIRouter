using System.Text.Json;
using ModelContextProtocol.Client;

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
}
