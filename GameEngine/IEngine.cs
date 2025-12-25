namespace GameEngine;

/// <summary>
/// A generic game engine.
/// </summary>
/// <typeparam name="TSharedState">The type of the state that is visible to all players.</typeparam>
/// <typeparam name="TPlayerState">The type of the state that each player privately has.</typeparam>
/// <typeparam name="TGameMove">The type of available game moves.</typeparam>
public interface IEngine<TSharedState, TPlayerState, TGameMove>
{
    /// <summary>
    /// Gets the players in the game.
    /// </summary>

    IReadOnlyList<IPlayer<TSharedState, TPlayerState, TGameMove>> Players { get; }

    /// <summary>
    /// Gets the shared state that is visible to all players.
    /// </summary>
    TSharedState State { get; }

    /// <summary>
    /// Occurs when the game state is updated.
    /// </summary>
    event EventHandler? Updated;

    /// <inheritdoc cref="IState{TGameState, TSharedState, TPlayerState, TGameMove}.IsValidMove(TGameMove, int?)"/>
    bool IsValidMove(TGameMove move, int? player = null);
    /// <inheritdoc cref="IState{TGameState, TSharedState, TPlayerState, TGameMove}.PlayMove(TGameMove, int?)"/>
    void PlayMove(TGameMove move, int? playerId = null);
}
