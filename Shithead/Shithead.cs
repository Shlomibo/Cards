using GameEngine;

using Shithead.State;

namespace Shithead;

public static class ShitheadGame
{
    public static Engine<
        ShitheadState,
        ShitheadState.SharedShitheadState,
        ShitheadState.ShitheadPlayerState,
                ShitheadMove.ShitheadMove> CreateGame(int playersCount)
    {
        return new Engine<
                        ShitheadState,
                        ShitheadState.SharedShitheadState,
                        ShitheadState.ShitheadPlayerState,
                        ShitheadMove.ShitheadMove>(new ShitheadState(playersCount));
    }
}
