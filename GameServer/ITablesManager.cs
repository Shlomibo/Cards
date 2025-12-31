using DTOs;


using System.Diagnostics.CodeAnalysis;

namespace GameServer;

/// <summary>
/// Manages game tables and player connections.
/// </summary>
/// <typeparam name="TInitOptions">Initialization options type.</typeparam>
/// <typeparam name="TGameState">The games state type.</typeparam>
/// <typeparam name="TSharedState">The shared state type.</typeparam>
/// <typeparam name="TPlayerState">The player-specific state type.</typeparam>
/// <typeparam name="TGameMove">The game move type.</typeparam>
/// <typeparam name="TSerializedState">The serialized state DTO type.</typeparam>
/// <typeparam name="TSerializedMove">The serialized move DTO type.</typeparam>
public interface ITablesManager<
        TInitOptions,
        TGameState,
        TSharedState,
        TPlayerState,
        TGameMove,
        TSerializedState,
        TSerializedMove>
        where TSerializedState : State
{
    /// <summary>
    /// Determines whether a player can join a specific table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <returns><c>true</c> if the player can join the table; otherwise, <c>false</c>.</returns>
    bool CanJoinTable(string tableName, string playerName);

    /// <summary>
    /// Creates a new gaming table.
    /// </summary>
    /// <param name="tableName">The name of the gaming-table to create.</param>
    /// <param name="tableMasterName">The name of the table master.</param>
    /// <returns>The connection of the table master to the newly created table.</returns>
    Connection<TGameState, TSharedState, TPlayerState, TGameMove, TSerializedState, TSerializedMove> CreateTable(
        string tableName,
        string tableMasterName);

    /// <summary>
    /// Gets a table by name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>The table.</returns>
    Table GetTable(string tableName);


    /// <summary>
    /// Joins a player to a specific table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <returns>The connection of the player to the table.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the table does not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a game was already started at the table.
    /// </exception>
    Connection<TGameState, TSharedState, TPlayerState, TGameMove, TSerializedState, TSerializedMove> JoinTable(
        string tableName,
        string playerName);

    /// <summary>
    /// Starts the game at a specific table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="masterConnectionId">The connection ID of the table master.</param>
    /// <param name="optionsFactory">Generates the initialization options for the game.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the table does not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the caller is not the table master.
    /// </exception>
    void StartGame(string tableName, Guid masterConnectionId, Func<Table, TInitOptions> optionsFactory);

    /// <summary>
    /// Tries to get a table by name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="table">
    /// When this method returns, contains the table if found; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the table was found; otherwise, <c>false</c>.</returns>
    bool TryGetTable(string tableName, [MaybeNullWhen(false)] out Table table);

    /// <summary>
    /// Tries to join a player to a specific table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="connection">
    /// When this method returns, contains the connection if the join was successful;
    /// otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the player successfully joined the table; otherwise, <c>false</c>.</returns>
    bool TryJoinTable(string tableName, string playerName, [MaybeNullWhen(false)] out Connection<TGameState, TSharedState, TPlayerState, TGameMove, TSerializedState, TSerializedMove> connection);
}
