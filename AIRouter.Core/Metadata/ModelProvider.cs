namespace AIRouter.Core.Metadata;

internal class ModelProvider
{
    public string Name { get; set; } = default!;

    public string Code { get; set; } = default!;

    public ModelProviderType Type { get; set; }

    public string ApiKey { get; set; } = string.Empty;

    public string? Endpoint { get; set; }

    public List<ModelProviderService> Services { get; set; } = [];

    public ModelProviderService? GetChatCompletionService()
    {
        return Services.FirstOrDefault(x => x.Type == ModelServiceType.ChatCompletion);
    }

    public string? GetChatCompletionModelId()
    {
        return GetChatCompletionService()?.ModelId;
    }

    public ModelProviderService? GetTextEmbeddingService()
    {
        return Services.FirstOrDefault(x => x.Type == ModelServiceType.TextEmbedding);
    }

    public string? GetTextEmbeddingModelId()
    {
        return GetTextEmbeddingService()?.ModelId;
    }
}

internal record ModelProviderService(ModelServiceType Type, string ModelId);
