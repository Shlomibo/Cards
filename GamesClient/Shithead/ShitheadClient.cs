using System;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GamesClient.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GamesClient.Shithead;

/// <summary>
/// Options for the Shithead client.
/// </summary>
public record ShitheadClientOptions : GameClientOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShitheadClientOptions"/> class.
    /// </summary>
    public ShitheadClientOptions()
        : base("shithead")
    {
    }
}

/// <summary>
/// A client for connecting to a Shithead game server.
/// </summary>
public sealed class ShitheadClient :
    GameClient<ShitheadClientOptions, ShitheadGameState, ShitheadMove>,
    IShitheadClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShitheadClient"/> class.
    /// </summary>
    /// <param name="options">The options for the Shithead client.</param>
    /// <param name="logger">The logger for the Shithead client.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    public ShitheadClient(
        IOptions<ShitheadClientOptions> options,
        ILogger<ShitheadClient> logger,
        IHttpClientFactory httpClientFactory)
        : base(options, logger, httpClientFactory)
    {
    }
}
