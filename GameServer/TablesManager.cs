using DTOs;

using GameEngine;


using System.Diagnostics.CodeAnalysis;

namespace GameServer;

/// <inheritdoc cref="ITablesManager{TInitOptions, TGameState, TSharedState, TPlayerState, TGameMove, TSerializedState, TSerializedMove}"/>
public class TablesManager<
    TInitOptions,
    TGameState,
    TSharedState,
    TPlayerState,
    TGameMove,
    TSerializedState,
    TSerializedMove> : ITablesManager<
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool CanJoinTable(string tableName, string playerName) =>
        !string.IsNullOrEmpty(tableName)
        && !string.IsNullOrEmpty(playerName)
        && Tables.TryGetValue(tableName, out var table)
        && table.CanAddPlayer(playerName);

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void StartGame(string tableName, Guid masterConnectionId, Func<Table, TInitOptions> optionsFactory)
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
            table.SetGame(_engineFactory(optionsFactory(table.AsTableDescriptor())));
        }
    }

    /// <inheritdoc/>
    public Table GetTable(string tableName) =>
        Tables[tableName].AsTableDescriptor();

    /// <inheritdoc/>
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
