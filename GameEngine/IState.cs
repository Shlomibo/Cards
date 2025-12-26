namespace GameEngine;

/// <summary>
/// Represents the state of the game.
/// </summary>
/// <typeparam name="TGameState">The type of the internal state of the game.</typeparam>
/// <typeparam name="TSharedState">The type of the state that is visible to all players.</typeparam>
/// <typeparam name="TPlayerState">The type of the state that each player privately has.</typeparam>
/// <typeparam name="TGameMove">The type of available game moves.</typeparam>
public interface IState<TGameState, TSharedState, TPlayerState, TGameMove>
{
    /// <summary>
    /// Gets the number of players in the game.
    /// </summary>
    int PlayersCount { get; }

    /// <summary>
    /// Gets the internal state of the game.
    /// </summary>
    TGameState GameState { get; }

    /// <summary>
    /// Gets the shared state that is visible to all players.
    /// </summary>
    TSharedState SharedState { get; }

    /// <summary>
    /// Determines whether the game is over.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the game is over.<br/>
    /// Otherwise, <see langword="false"/>.
    /// </returns>
    bool IsGameOver();

    /// <summary>
    /// Gets the state of the specified player.
    /// </summary>
    /// <param name="player">The id of the player to get its state.</param>
    /// <returns>The state of the specified player.</returns>
    TPlayerState GetPlayerState(int player);

    /// <summary>
    /// Determines whether the specified <paramref name="move"/> is valid for the specified <paramref name="player"/>.
    /// </summary>
    /// <param name="move">The move to check.</param>
    /// <param name="player">The player who acts.</param>
    /// <returns>
    /// <see langword="true"/> if the move is valid to play.<br/>
    /// Otherwise, <see langword="false"/>.
    /// </returns>
    bool IsValidMove(TGameMove move, int? player = null);

    /// <summary>
    /// Plays the specified <paramref name="move"/> for the specified <paramref name="player"/>.
    /// </summary>
    /// <param name="move">The move to play.</param>
    /// <param name="player">The player who acts.</param>
    /// <returns>
    /// <see langword="true"/> if the move was played successfully.<br/>
    /// Otherwise, <see langword="false"/>.
    /// </returns>
    bool PlayMove(TGameMove move, int? player = null);

    /// <summary>
    /// Removes the specified <paramref name="player"/> from the game.
    /// </summary>
    /// <param name="player">The player to remove.</param>
    void RemovePlayer(int player);
}
