namespace AIRouter.Core.Metadata;

internal class ModelProviderOptions
{
    public string DefaultProvider { get; set; } = "OpenAI";

    public List<ModelProvider> Providers { get; set; } = [];

    public ModelProvider? GetDefaultProvider()
    {
        return GetProvider(DefaultProvider);
    }

    public ModelProvider? GetProvider(string code)
    {
        return Providers.FirstOrDefault(x => x.Code == code);
    }
}
