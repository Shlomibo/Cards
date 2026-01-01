using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GamesClient.Shithead;

public static class DiExtensions
{
    public static IServiceCollection AddShitheadClient(
        this IServiceCollection services,
        string configSection = "")
    {
        var optionsBinding = services.AddOptions<ShitheadClientOptions>();

        optionsBinding = configSection == ""
            ? optionsBinding.Configure<IConfigurationRoot>((options, config) =>
                config.Bind(options))
            : optionsBinding.BindConfiguration(configSection);

        optionsBinding
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient(nameof(ShitheadClient), (services, client) =>
        {
            var config = services.GetRequiredService<IOptions<ShitheadClientOptions>>();
            client.BaseAddress = config.Value.BaseUrl;
        });

        services.TryAddSingleton<IShitheadClient, ShitheadClient>();

        return services;
    }
}
