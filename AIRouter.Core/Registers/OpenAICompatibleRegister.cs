﻿using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using AIRouter.Core.ClientHandlers;
using AIRouter.Core.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OpenAI;

namespace AIRouter.Core.Registers;

internal class OpenAICompatibleRegister : ModelProviderRegisterBase
{
    public override ModelProviderType ProviderType => ModelProviderType.OpenAI_Compatible;

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

        builder.AddOpenAIChatCompletion(
            modelId: modelId,
            openAIClient: new OpenAIClient(
                new ApiKeyCredential(provider.ApiKey),
                new OpenAIClientOptions { Endpoint = new Uri(provider.Endpoint!) }
            )
        );

        builder.AddOpenAIChatCompletion(
            modelId: modelId,
            apiKey: provider.ApiKey,
            httpClient: new HttpClient(
                new OpenAIHttpClientHandler(
                    sp.GetRequiredService<ILoggerFactory>().CreateLogger<OpenAIHttpClientHandler>()
                )
            )
        );
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

        var openAIClient = new OpenAIClient(
            new ApiKeyCredential(provider.ApiKey),
            new OpenAIClientOptions { Endpoint = new Uri(provider.Endpoint!), }
        );
        builder.AddOpenAITextEmbeddingGeneration(modelId: modelId, openAIClient: openAIClient);
    }
}
