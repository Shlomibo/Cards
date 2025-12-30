using System;
using Games.Services.Shithead;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Games;

public static class DIExtensions
{
    public static IServiceCollection AddGames(this IServiceCollection services)
    {
        services.TryAddSingleton<IShitheadTablesManager, ShitheadTablesManager>();

        return services;
    }
}
