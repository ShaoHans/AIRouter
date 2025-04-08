using AIRouter.Core;

namespace Microsoft.Extensions.Configuration;

internal static class ConfigurationExtensions
{
    public static ModelProviderOptions ConfigModelProvoiderOptions(
        this IConfiguration configuration
    )
    {
        var options = configuration.GetSection("AIModelOptions").Get<ModelProviderOptions>();

        return options is null
            ? throw new ArgumentException("not found AIModelOptions on configuration")
            : options!;
    }
}
