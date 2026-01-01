using System;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GamesClient.Client;

namespace GamesClient.Shithead;

public interface IShitheadClient : IClient<ShitheadGameState, ShitheadMove>;
