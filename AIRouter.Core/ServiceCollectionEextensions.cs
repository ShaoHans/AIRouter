using AIRouter.Core.Registers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionEextensions
{
    //public static T? GetSingletonInstanceOrNull<T>(this IServiceCollection services)
    //{
    //    return (T?)services.FirstOrDefault(d => d.ServiceType == typeof(T))?.ImplementationInstance;
    //}

    //public static IConfiguration? GetConfiguration(this IServiceCollection services)
    //{
    //    var hostBuilderContext = services.GetSingletonInstanceOrNull<HostBuilderContext>();
    //    if (hostBuilderContext?.Configuration != null)
    //    {
    //        return hostBuilderContext.Configuration as IConfigurationRoot;
    //    }

    //    return services.GetSingletonInstanceOrNull<IConfiguration>();
    //}

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

    public static IServiceCollection AddSerilog(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();

            //var configuration = services.GetConfiguration();
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            builder.AddSerilog(logger);
        });
        return services;
    }
}
