namespace AIRouter.Core;

internal class ModelProvider
{
    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public ModelProviderType Type { get; set; }

    public string ApiKey { get; set; } = default!;

    public string? Endpoint { get; set; }

    public List<ModelProviderService> Services { get; set; } = [];

    public ModelProviderService? GetChatCompletionService()
    {
        return Services.FirstOrDefault(x => x.Type == ModelServiceType.ChatCompletion);
    }

    public ModelProviderService? GetTextEmbeddingService()
    {
        return Services.FirstOrDefault(x => x.Type == ModelServiceType.TextEmbedding);
    }
}

internal record ModelProviderService(ModelServiceType Type, string ModelId);
