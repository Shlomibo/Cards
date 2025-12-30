using DTOs.Shithead;
using DTOs.Shithead.Moves;
using GameServer;
using Shithead;
using Shithead.Moves;
using Shithead.State;

namespace Games.Services.Shithead;

public sealed class ShitheadTablesManager :
    TablesManager<
        InitOptions,
        ShitheadState,
        ShitheadState.SharedShitheadState,
        ShitheadState.ShitheadPlayerState,
        Move,
        ShitheadGameState,
        ShitheadMove>,
    IShitheadTablesManager
{
    public ShitheadTablesManager()
        : base(
            options => new ShitheadEngine(
                new ShitheadState(options.PlayersCount)),
            SerializeState,
            DeserializeMove)
    {
    }

    private static ShitheadGameState SerializeState(
        ShitheadState.SharedShitheadState sharedState,
        ShitheadState.ShitheadPlayerState playerState)
        =>
        StateSerialization.Map(sharedState, playerState);

    private static Move DeserializeMove(ShitheadMove move) =>
        MoveDeserialization.Map(move);
}
