using AIRouter.Core.Metadata;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

namespace Microsoft.SemanticKernel;

public static class KernelBuilderExtensions
{
    public static IKernelBuilder AddChatCompletion(
        this IKernelBuilder builder,
        IConfiguration configuration,
        string? providerCode = null
    )
    {
        var options = configuration.ConfigModelProvoiderOptions();
        var provider =
            (
                string.IsNullOrWhiteSpace(providerCode)
                    ? options.GetDefaultProvider()
                    : options.GetProvider(providerCode!)
            )
            ?? throw new ArgumentException(
                $"not found {options.DefaultProvider} or {providerCode} ai model provider configuration"
            );

        var modelId = provider.GetChatCompletionService()?.ModelId;
        if (string.IsNullOrWhiteSpace(modelId))
        {
            throw new ArgumentException(
                $"the {provider.Name} provider not found {ModelServiceType.ChatCompletion} service model"
            );
        }

        switch (provider.Type)
        {
            case ModelProviderType.OpenAI:
                builder.AddOpenAIChatCompletion(provider.ApiKey, modelId);
                break;
            case ModelProviderType.Ollama:
            case ModelProviderType.OpenAI_Compatible:
                {
                    var openAIClient = new OpenAIClient(
                        new ApiKeyCredential(provider.ApiKey),
                        new OpenAIClientOptions { Endpoint = new Uri(provider.Endpoint!), }
                    );
                    builder.AddOpenAIChatCompletion(modelId, openAIClient);
                }
                break;
            case ModelProviderType.AzureOpenAI:
                builder.AddAzureOpenAIChatCompletion(
                    deploymentName: modelId,
                    apiKey: provider.ApiKey,
                    endpoint: provider.Endpoint!
                );
                break;
            //case ModelProviderType.Ollama:
            //    builder.AddOpenAIChatCompletion(
            //        modelId,
            //        provider.ApiKey,
            //        httpClient: new HttpClient(new OllamaHttpClientHandler(provider.Endpoint!))
            //    );
            //    break;
            case ModelProviderType.HuggingFace:
            case ModelProviderType.Google:
            case ModelProviderType.Onnx:
            case ModelProviderType.MistralAI:
            default:
                throw new NotSupportedException(
                    $"the {provider.Name} provider not support {ModelServiceType.ChatCompletion} service"
                );
        }

        return builder;
    }
}
