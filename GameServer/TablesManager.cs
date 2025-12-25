using DTOs;

using GameEngine;


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
public class TablesManager<
    TInitOptions,
    TGameState,
    TSharedState,
    TPlayerState,
    TGameMove,
    TSerializedState,
    TSerializedMove>
    where TSerializedState : State
{
    private readonly Func<TInitOptions, IEngine<TSharedState, TPlayerState, TGameMove>>
        _engineFactory;
    private readonly Func<TSharedState, TPlayerState, TSerializedState> _stateSerializer;
    private readonly Func<TSerializedMove, TGameMove> _moveDeserializer;
    internal Dictionary<string, ITable<TGameState, TSharedState, TPlayerState, TGameMove>> Tables { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TablesManager{TInitOptions, TGameState, TSharedState, TPlayerState, TGameMove, TSerializedState, TSerializedMove}"/> class.
    /// </summary>
    /// <param name="engineFactory">A factory function to create game engines.</param>
    /// <param name="stateSerializer">A function to serialize game states.</param>
    /// <param name="moveDeserializer">A function to deserialize game moves.</param>
    public TablesManager(
        Func<TInitOptions, IEngine<TSharedState, TPlayerState, TGameMove>> engineFactory,
        Func<TSharedState, TPlayerState, TSerializedState> stateSerializer,
        Func<TSerializedMove, TGameMove> moveDeserializer)
        : this(engineFactory, stateSerializer, moveDeserializer, [])
    {
    }

    internal TablesManager(
        Func<TInitOptions, IEngine<TSharedState, TPlayerState, TGameMove>> engineFactory,
        Func<TSharedState, TPlayerState, TSerializedState> stateSerializer,
        Func<TSerializedMove, TGameMove> moveDeserializer,
        IEnumerable<KeyValuePair<string, ITable<TGameState, TSharedState, TPlayerState, TGameMove>>> tables)
    {
        _engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
        _stateSerializer = stateSerializer ?? throw new ArgumentNullException(nameof(stateSerializer));
        _moveDeserializer = moveDeserializer ?? throw new ArgumentNullException(nameof(moveDeserializer));
        Tables = tables?.ToDictionary() ?? throw new ArgumentNullException(nameof(tables));
    }

    /// <summary>
    /// Creates a new gaming table.
    /// </summary>
    /// <param name="tableName">The name of the gaming-table to create.</param>
    /// <param name="tableMasterName">The name of the table master.</param>
    /// <returns>The connection of the table master to the newly created table.</returns>
    public Connection<
        TGameState,
        TSharedState,
        TPlayerState,
        TGameMove,
        TSerializedState,
        TSerializedMove> CreateTable(string tableName, string tableMasterName)
    {
        ArgumentException.ThrowIfNullOrEmpty(tableName);
        ArgumentException.ThrowIfNullOrEmpty(tableMasterName);

        if (Tables.ContainsKey(tableName))
        {
            throw new InvalidOperationException($"The table '{tableName}' already exists");
        }

        Table<TGameState, TSharedState, TPlayerState, TGameMove> table = new(tableName, tableMasterName);
        Tables.Add(tableName, table);

        return CreateConnection(table, table.TableMaster.ConnectionId);
    }

    /// <summary>
    /// Determines whether a player can join a specific table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="playerName">The name of the player.</param>
    /// <returns><c>true</c> if the player can join the table; otherwise, <c>false</c>.</returns>
    public bool CanJoinTable(string tableName, string playerName) =>
        !string.IsNullOrEmpty(tableName)
        && !string.IsNullOrEmpty(playerName)
        && Tables.TryGetValue(tableName, out var table)
        && table.CanAddPlayer(playerName);

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
    public bool TryJoinTable(
        string tableName,
        string playerName,
        [MaybeNullWhen(false)] out Connection<
            TGameState,
            TSharedState,
            TPlayerState,
            TGameMove,
            TSerializedState,
            TSerializedMove> connection)
    {
        connection = null;

        if (CanJoinTable(tableName, playerName))
        {
            try
            {
                connection = JoinTable(tableName, playerName);
            }
            catch
            {
                return false;
            }
        }

        return connection != null;
    }

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
    public Connection<
        TGameState,
        TSharedState,
        TPlayerState,
        TGameMove,
        TSerializedState,
        TSerializedMove> JoinTable(string tableName, string playerName)
    {
        ArgumentException.ThrowIfNullOrEmpty(tableName);
        ArgumentException.ThrowIfNullOrEmpty(playerName);

        if (!Tables.TryGetValue(tableName, out var table))
        {
            throw new InvalidOperationException($"The table '{tableName}' does not exist");
        }

        if (table.GameStarted)
        {
            throw new InvalidOperationException("A game was already started");
        }

        var player = table.AddPlayer(playerName);

        return CreateConnection(table, player.ConnectionId);
    }

    /// <summary>
    /// Starts the game at a specific table.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="masterConnectionId">The connection ID of the table master.</param>
    /// <param name="options">The initialization options for the game.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the table does not exist.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the caller is not the table master.
    /// </exception>
    public void StartGame(string tableName, Guid masterConnectionId, TInitOptions options)
    {
        if (!Tables.TryGetValue(tableName, out var table))
        {
            throw new ArgumentException($"Cannot find table [{tableName}]", nameof(tableName));
        }

        if (masterConnectionId != table.TableMaster.ConnectionId)
        {
            throw new InvalidOperationException("Only the table master can start a game");
        }

        if (!table.GameStarted)
        {
            table.SetGame(_engineFactory(options));
        }
    }

    /// <summary>
    /// Gets a table by name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>The table.</returns>
    public Table GetTable(string tableName) =>
        Tables[tableName].AsTableDescriptor();

    /// <summary>
    /// Tries to get a table by name.
    /// </summary>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="table">
    /// When this method returns, contains the table if found; otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the table was found; otherwise, <c>false</c>.</returns>
    public bool TryGetTable(string tableName, [MaybeNullWhen(false)] out Table table)
    {
        table = null;

        if (tableName == null)
        {
            return false;
        }

        bool hasTable = Tables.TryGetValue(tableName, out var internalTable);

        if (hasTable)
        {
            table = internalTable!.AsTableDescriptor();
        }

        return hasTable;
    }

    private Connection<
        TGameState,
        TSharedState,
        TPlayerState,
        TGameMove,
        TSerializedState,
        TSerializedMove> CreateConnection(
            ITable<TGameState, TSharedState, TPlayerState, TGameMove> table,
            Guid connectionId)
    {
        var connection = new Connection<
            TGameState,
            TSharedState,
            TPlayerState,
            TGameMove,
            TSerializedState,
            TSerializedMove>(table, connectionId, _stateSerializer, _moveDeserializer);

        return connection;
    }
}
