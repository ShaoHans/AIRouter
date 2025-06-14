﻿using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Client;

namespace AIRouter.Console.MCP;

internal static class AIFunctionExtensions
{
    internal static async Task<IEnumerable<KernelFunction>> MapToFunctionsAsync(
        this IMcpClient mcpClient
    )
    {
        var tools = await mcpClient.ListToolsAsync().ConfigureAwait(false);
        return tools.Select(t => t.ToKernelFunction(mcpClient)).ToList();
    }

    private static KernelFunction ToKernelFunction(this AIFunction tool, IMcpClient mcpClient)
    {
        async Task<string> InvokeToolAsync(
            Kernel kernel,
            KernelFunction function,
            KernelArguments arguments,
            CancellationToken cancellationToken
        )
        {
            try
            {
                // Convert arguments to dictionary format expected by ModelContextProtocol
                Dictionary<string, object> mcpArguments = [];
                foreach (var arg in arguments)
                {
                    if (arg.Value is not null)
                    {
                        mcpArguments[arg.Key] = function.ToArgumentValue(arg.Key, arg.Value);
                    }
                }
                // Call the tool through ModelContextProtocol
                var result = await mcpClient
                    .CallToolAsync(tool.Name, mcpArguments, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                // Extract the text content from the result
                return string.Join(
                    "\n",
                    result.Content.Where(c => c.Type == "text").Select(c => c.Text)
                );
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"Error invoking tool '{tool.Name}': {ex.Message}");
                // Rethrowing to allow the kernel to handle the exception
                throw;
            }
        }
        return KernelFunctionFactory.CreateFromMethod(
            method: InvokeToolAsync,
            functionName: tool.Name,
            description: tool.Description,
            parameters: tool.ToParameters(),
            returnParameter: ToReturnParameter()
        );
    }

    private static object ToArgumentValue(this KernelFunction function, string name, object value)
    {
        var parameter = function.Metadata.Parameters.FirstOrDefault(p => p.Name == name);
        return parameter?.ParameterType switch
            {
                Type t when Nullable.GetUnderlyingType(t) == typeof(int) => Convert.ToInt32(value),
                Type t when Nullable.GetUnderlyingType(t) == typeof(double)
                    => Convert.ToDouble(value),
                Type t when Nullable.GetUnderlyingType(t) == typeof(bool)
                    => Convert.ToBoolean(value),
                Type t when t == typeof(List<string>) => (value as IEnumerable<object>)?.ToList(),
                Type t when t == typeof(Dictionary<string, object>)
                    => (value as Dictionary<string, object>)?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value
                    ),
                _ => value,
            } ?? value;
    }

    private static List<KernelParameterMetadata>? ToParameters(this AIFunction tool)
    {
        var inputSchema = JsonSerializer.Deserialize<JsonSchema>(tool.JsonSchema);
        var properties = inputSchema?.Properties;
        if (properties == null)
        {
            return null;
        }
        HashSet<string> requiredProperties = new(inputSchema!.Required ?? []);
        return properties
            .Select(kvp => new KernelParameterMetadata(kvp.Key)
            {
                Description = kvp.Value.Description,
                ParameterType = ConvertParameterDataType(
                    kvp.Value,
                    requiredProperties.Contains(kvp.Key)
                ),
                IsRequired = requiredProperties.Contains(kvp.Key)
            })
            .ToList();
    }

    private static KernelReturnParameterMetadata? ToReturnParameter()
    {
        return new KernelReturnParameterMetadata() { ParameterType = typeof(string), };
    }

    private static Type ConvertParameterDataType(JsonSchemaProperty property, bool required)
    {
        var type = property.Type switch
        {
            "string" => typeof(string),
            "integer" => typeof(int),
            "number" => typeof(double),
            "boolean" => typeof(bool),
            "array" => typeof(List<string>),
            "object" => typeof(Dictionary<string, object>),
            _ => typeof(object)
        };
        return !required && type.IsValueType ? typeof(Nullable<>).MakeGenericType(type) : type;
    }

    public class JsonSchema
    {
        /// <summary>
        /// The type of the schema, should be "object".
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        /// <summary>
        /// Map of property names to property definitions.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("properties")]
        public Dictionary<string, JsonSchemaProperty>? Properties { get; set; }

        /// <summary>
        /// List of required property names.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("required")]
        public List<string>? Required { get; set; }
    }

    public class JsonSchemaProperty
    {
        /// <summary>
        /// The type of the property. Should be a JSON Schema type and is required.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// A human-readable description of the property.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string? Description { get; set; } = string.Empty;
    }
}
