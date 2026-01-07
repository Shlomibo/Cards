using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GamesClient.Shithead;

/// <summary>
/// Dependency injection extensions for the Shithead client.
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Adds the Shithead client to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configSection">The configuration section for the Shithead client options.</param>
    /// <returns>The updated service collection.</returns>
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
