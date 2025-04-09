using AIRouter.Core.Registers;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionEextensions
{
    public static IServiceCollection RegisterKernels(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var options = configuration.ConfigModelProvoiderOptions();
        foreach (var provider in options.Providers)
        {
            var providerRegister = ModelProviderRegisterFactory.Create(provider!.Type);
            providerRegister.Register(services, provider);
        }
        return services;
    }
}
