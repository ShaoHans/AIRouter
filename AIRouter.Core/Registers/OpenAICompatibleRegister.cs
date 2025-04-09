using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using AIRouter.Core.Metadata;
using Microsoft.SemanticKernel;
using OpenAI;

namespace AIRouter.Core.Registers;

internal class OpenAICompatibleRegister : ModelProviderRegisterBase
{
    public override ModelProviderType ProviderType => ModelProviderType.OpenAI_Compatible;

    protected override void AddChatCompletionService(IKernelBuilder builder, ModelProvider provider)
    {
        var modelId = provider.GetChatCompletionModelId();
        if (string.IsNullOrWhiteSpace(modelId))
        {
            return;
        }

        var openAIClient = new OpenAIClient(
            new ApiKeyCredential(provider.ApiKey),
            new OpenAIClientOptions { Endpoint = new Uri(provider.Endpoint!), }
        );
        builder.AddOpenAIChatCompletion(modelId: modelId, openAIClient: openAIClient);
    }

    [Experimental("SKEXP0010")]
    protected override void AddTextEmbeddingService(IKernelBuilder builder, ModelProvider provider)
    {
        var modelId = provider.GetTextEmbeddingModelId();
        if (string.IsNullOrWhiteSpace(modelId))
        {
            return;
        }

        var openAIClient = new OpenAIClient(
            new ApiKeyCredential(provider.ApiKey),
            new OpenAIClientOptions { Endpoint = new Uri(provider.Endpoint!), }
        );
        builder.AddOpenAITextEmbeddingGeneration(modelId: modelId, openAIClient: openAIClient);
    }
}
