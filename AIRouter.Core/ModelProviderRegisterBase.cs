using AIRouter.Core.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace AIRouter.Core;

internal abstract class ModelProviderRegisterBase
{
    public abstract ModelProviderType ProviderType { get; }

    public virtual void Register(IServiceCollection services, ModelProvider provider)
    {
        services.AddKeyedTransient(provider.Code, (sp, key) => BuildKernel(sp, provider));
    }

    protected abstract void RegisterChatCompletionService(
        IKernelBuilder builder,
        ModelProvider provider
    );

    protected abstract void RegisterTextEmbeddingService(
        IKernelBuilder builder,
        ModelProvider provider
    );

    public virtual Kernel BuildKernel(IServiceProvider sp, ModelProvider provider)
    {
        var builder = Kernel.CreateBuilder();

        RegisterChatCompletionService(builder, provider);
        RegisterTextEmbeddingService(builder, provider);

        return builder.Build();
    }
}
