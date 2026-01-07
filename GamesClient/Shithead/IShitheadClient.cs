using System;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GamesClient.Client;

namespace GamesClient.Shithead;

/// <summary>
/// A client for connecting to a Shithead game server.
/// </summary>
public interface IShitheadClient : IClient<ShitheadGameState, ShitheadMove>;
