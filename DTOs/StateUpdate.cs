namespace DTOs;

/// <summary>
/// A DTO representing a state update in the game
/// </summary>
/// <param name="TableName">The name of the gaming-table for which the state update is.</param>
/// <param name="CurrentPlayer">The current player.</param>
/// <param name="Table">A dictionary of the other players around the table.</param>
/// <param name="State">The current state of the game, if it has started.</param>
/// <typeparam name="TState">The type of game's state.</typeparam>
public sealed record StateUpdate<TState>(
    string TableName,
    CurrentPlayer CurrentPlayer,
    IReadOnlyDictionary<int, Player> Table,
    TState? State = null)
    where TState : State
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StateUpdate{TState}"/> record.
    /// </summary>
    /// <param name="tableName">The name of the gaming-table for which the state update is.</param>
    /// <param name="currentPlayer">The current player.</param>
    /// <param name="players">The other players around the table.</param>
    /// <param name="state">The current state of the game, if it has started.</param>
    public StateUpdate(
        string tableName,
        CurrentPlayer currentPlayer,
        IEnumerable<Player> players,
        TState? state = null)
        : this(
            tableName,
            currentPlayer,
            players.ToDictionary(player => player.PlayerId),
            state)
    {
    }
}

/// <summary>
/// A player around a gaming-table.
/// </summary>
/// <param name="PlayerId">The id of the player.</param>
/// <param name="PlayerName">The display name of the player.</param>
/// <param name="State">The state of the player.</param>
public sealed record Player(int PlayerId, string PlayerName, PlayerState State);

/// <summary>
/// The current player.
/// </summary>
/// <param name="PlayerId">The id of the player.</param>
/// <param name="PlayerName">The display name of the player.</param>
/// <param name="ConnectionId">A unique secret that identifies the current user.</param>
public sealed record CurrentPlayer(int PlayerId, string PlayerName, Guid ConnectionId);

/// <summary>
/// The state of a <see cref="Player"/>.
/// </summary>
public enum PlayerState
{
    /// <summary>
    /// The player is playing the game.
    /// </summary>
    Playing,

    /// <summary>
    /// The player has left the game.
    /// </summary>
    LeftGame,
}
