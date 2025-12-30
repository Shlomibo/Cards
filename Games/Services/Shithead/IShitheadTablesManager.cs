using System;
using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GameServer;
using Shithead.Moves;
using Shithead.State;

namespace Games.Services.Shithead;

public interface IShitheadTablesManager : ITablesManager<
    InitOptions,
    ShitheadState,
    ShitheadState.SharedShitheadState,
    ShitheadState.ShitheadPlayerState,
    Move,
    ShitheadGameState,
    ShitheadMove>;
