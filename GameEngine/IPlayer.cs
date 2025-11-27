namespace GameEngine;

/// <summary>
/// Player's view of the game.
/// </summary>
/// <typeparam name="TSharedState">
/// The type of the shared state. That is the state which is visible to all players.
/// </typeparam>
/// <typeparam name="TState">
/// The player's state. That is is the state which is visible only to current player.
/// </typeparam>
/// <typeparam name="TGameMove">
/// The type of the game move.
/// </typeparam>
public interface IPlayer<TSharedState, TState, TGameMove>
{
    /// <summary>
    /// Occurs when the game state is updated.
    /// </summary>
    event EventHandler Updated;

    /// <summary>
    /// Gets the shared state that is visible to all players.
    /// </summary>
    TSharedState SharedState { get; }

    /// <summary>
    /// Gets the player's state that is visible only to the current player.
    /// </summary>
    TState State { get; }

    /// <summary>
    /// Gets the current player's identifier.
    /// </summary>
    /// <value></value>
    int PlayerId { get; }

    //TGameMove Actions { get; }

    /// <summary>
    /// Plays the specified move.
    /// </summary>
    /// <param name="move">The move to play.</param>
    void PlayMove(TGameMove move);

    /// <summary>
    /// Determines whether the specified move is valid.
    /// </summary>
    /// <param name="move">The move to check.</param>
    /// <returns>
    /// <see langword="true"/> if the move is valie to play.<br/>
    /// Otherwise, <see langword="false"/>.
    /// </returns>
    bool IsValidMove(TGameMove move);
}
