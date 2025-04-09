using AIRouter.Core.Metadata;

namespace AIRouter.Core.Registers;

internal static class ModelProviderRegisterFactory
{
    public static ModelProviderRegisterBase Create(ModelProviderType providerType)
    {
        return providerType switch
        {
            ModelProviderType.OpenAI => new OpenAIRegister(),
            ModelProviderType.AzureOpenAI => new AzureOpenAIRegister(),
            ModelProviderType.OpenAI_Compatible or ModelProviderType.Ollama => new OpenAICompatibleRegister(),
            _ => throw new NotSupportedException($"Model provider type {providerType} is not supported.")
        };
    }
}
