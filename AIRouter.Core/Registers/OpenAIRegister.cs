﻿using System.Diagnostics.CodeAnalysis;
using AIRouter.Core.Metadata;
using Microsoft.SemanticKernel;

namespace AIRouter.Core.Registers;

internal class OpenAIRegister : ModelProviderRegisterBase
{
    public override ModelProviderType ProviderType => ModelProviderType.OpenAI;

    protected override void AddChatCompletionService(
        IKernelBuilder builder,
        IServiceProvider sp,
        ModelProvider provider
    )
    {
        var modelId = provider.GetChatCompletionModelId();
        if (string.IsNullOrWhiteSpace(modelId))
        {
            return;
        }

        builder.AddOpenAIChatCompletion(modelId: modelId, apiKey: provider.ApiKey);
    }

    [Experimental("SKEXP0010")]
    protected override void AddTextEmbeddingService(
        IKernelBuilder builder,
        IServiceProvider sp,
        ModelProvider provider
    )
    {
        var modelId = provider.GetTextEmbeddingModelId();
        if (string.IsNullOrWhiteSpace(modelId))
        {
            return;
        }

        builder.AddOpenAIEmbeddingGenerator(modelId: modelId, apiKey: provider.ApiKey);
    }
}
