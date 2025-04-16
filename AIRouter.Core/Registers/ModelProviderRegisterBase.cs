using AIRouter.Core.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace AIRouter.Core.Registers;

internal abstract class ModelProviderRegisterBase
{
    public abstract ModelProviderType ProviderType { get; }

    public virtual void Register(
        IServiceCollection services,
        IConfiguration configuration,
        ModelProvider provider
    )
    {
        services.AddKeyedTransient(
            provider.Code,
            (sp, key) => BuildKernel(sp, configuration, provider)
        );
    }

    protected abstract void AddChatCompletionService(
        IKernelBuilder builder,
        IServiceProvider sp,
        ModelProvider provider
    );

    protected abstract void AddTextEmbeddingService(
        IKernelBuilder builder,
        IServiceProvider sp,
        ModelProvider provider
    );

    public virtual Kernel BuildKernel(
        IServiceProvider sp,
        IConfiguration configuration,
        ModelProvider provider
    )
    {
        var builder = Kernel.CreateBuilder();

        // 给Kernel注册Serilog组件，这样就可以通过kernel.Services.GetRequiredService<ILoggerProvider>()获取Serilog的ILogger
        builder.Services.AddSerilog(configuration);
        AddChatCompletionService(builder, sp, provider);
        AddTextEmbeddingService(builder, sp, provider);

        return builder.Build();
    }
}
