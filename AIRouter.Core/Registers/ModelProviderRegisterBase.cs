using AIRouter.Core.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace AIRouter.Core.Registers;

internal abstract class ModelProviderRegisterBase
{
    public abstract ModelProviderType ProviderType { get; }

    public virtual void Register(IServiceCollection services, ModelProvider provider)
    {
        services.AddKeyedTransient(provider.Code, (sp, key) => BuildKernel(sp, provider));
    }

    protected abstract void AddChatCompletionService(
        IKernelBuilder builder,
        ModelProvider provider
    );

    protected abstract void AddTextEmbeddingService(
        IKernelBuilder builder,
        ModelProvider provider
    );

    public virtual Kernel BuildKernel(IServiceProvider sp, ModelProvider provider)
    {
        var builder = Kernel.CreateBuilder();

        AddChatCompletionService(builder, provider);
        AddTextEmbeddingService(builder, provider);

        return builder.Build();
    }
}
