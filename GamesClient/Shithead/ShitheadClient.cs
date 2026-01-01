using System;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GamesClient.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GamesClient.Shithead;

public record ShitheadClientOptions : GameClientOptions
{
    public ShitheadClientOptions()
        : base("shithead")
    {
    }
}

public sealed class ShitheadClient :
    GameClient<ShitheadClientOptions, ShitheadGameState, ShitheadMove>,
    IShitheadClient
{
    public ShitheadClient(
        IOptions<ShitheadClientOptions> options,
        ILogger<ShitheadClient> logger,
        HttpClient httpClient)
        : base(options, logger, httpClient)
    {
    }
}
