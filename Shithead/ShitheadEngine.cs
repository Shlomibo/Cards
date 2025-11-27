using System;

using GameEngine;

using Shithead.State;

namespace Shithead;

/// <summary>
/// A Shithead game engine.
/// </summary>
public sealed class ShitheadEngine :
    Engine<
        ShitheadState,
        ShitheadState.SharedShitheadState,
        ShitheadState.ShitheadPlayerState,
        Moves.Move>
{
    /// <inheritdoc cref="Engine{TGameState, TSharedState, TPlayerState, TGameMove}.Engine(IState{TGameState, TSharedState, TPlayerState, TGameMove})"/>
    public ShitheadEngine(ShitheadState state)
        : base(state)
    {

    }
}
