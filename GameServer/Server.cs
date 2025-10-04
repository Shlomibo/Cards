using GameEngine;
using GameServer.DTO;
using System.Diagnostics.CodeAnalysis;

namespace GameServer;

public class Server<
	TInitOptions,
	TGameState,
	TSharedState,
	TPlayerState,
	TGameMove,
	TSerializedState,
	TSerializedMove>
	where TSerializedState : class, IState<object, object>
	where TSerializedMove : IMove
{
	private readonly Func<TInitOptions, Engine<TGameState, TSharedState, TPlayerState, TGameMove>>
		engineFactory;
	private readonly Func<TSharedState, TPlayerState, TSerializedState> stateSerializer;
	private readonly Func<TSerializedMove, TGameMove> moveDeserializer;
	private readonly Dictionary<string, Table<TGameState, TSharedState, TPlayerState, TGameMove>> tables = new();

	public Server(
		Func<TInitOptions, Engine<TGameState, TSharedState, TPlayerState, TGameMove>> engineFactory,
		Func<TSharedState, TPlayerState, TSerializedState> stateSerializer,
		Func<TSerializedMove, TGameMove> moveDeserializer)
	{
		this.engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
		this.stateSerializer = stateSerializer ?? throw new ArgumentNullException(nameof(stateSerializer));
		this.moveDeserializer = moveDeserializer ?? throw new ArgumentNullException(nameof(moveDeserializer));
	}

	public Connection<
		TGameState,
		TSharedState,
		TPlayerState,
		TGameMove,
		TSerializedState,
		TSerializedMove> CreateTable(string tableName, string tableMasterName)
	{
		if (string.IsNullOrEmpty(tableName))
		{
			throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
		}

		if (string.IsNullOrEmpty(tableMasterName))
		{
			throw new ArgumentException($"'{nameof(tableMasterName)}' cannot be null or empty.", nameof(tableMasterName));
		}

		if (tables.ContainsKey(tableName))
		{
			throw new ArgumentException($"The table '{tableName}' already exists", nameof(tableName));
		}

		Table<TGameState, TSharedState, TPlayerState, TGameMove> table = new(tableName, tableMasterName);
		tables.Add(tableName, table);

		return CreateConnection(table, table.TableMaster.ConnectionId);
	}

	public bool CanJoinTable(string tableName, string playerName) =>
		!string.IsNullOrEmpty(tableName) &&
			!string.IsNullOrEmpty(playerName) &&
			tables.TryGetValue(tableName, out var table) &&
			!table.GameStarted;

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

	public Connection<
		TGameState,
		TSharedState,
		TPlayerState,
		TGameMove,
		TSerializedState,
		TSerializedMove> JoinTable(string tableName, string playerName)
	{
		if (string.IsNullOrEmpty(tableName))
		{
			throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
		}

		if (string.IsNullOrEmpty(playerName))
		{
			throw new ArgumentException($"'{nameof(playerName)}' cannot be null or empty.", nameof(playerName));
		}

		if (!tables.TryGetValue(tableName, out var table))
		{
			throw new ArgumentException($"The table '{tableName}' already exists", nameof(tableName));
		}

		if (table.GameStarted)
		{
			throw new InvalidOperationException("A game was already started");
		}

		var player = table.AddPlayer(playerName);

		return CreateConnection(table, player.ConnectionId);
	}

	public void StartGame(string tableName, Guid masterConnectionId, TInitOptions options)
	{
		if (!tables.TryGetValue(tableName, out var table))
		{
			throw new ArgumentException($"Cannot find table [{tableName}]", nameof(tableName));
		}

		if (masterConnectionId != table.TableMaster.ConnectionId)
		{
			throw new InvalidOperationException("Only the table master can start a game");
		}

		if (!table.GameStarted)
		{
			table.SetGame(engineFactory(options));
		}
	}

	public Table GetTable(string tableName) =>
		tables[tableName].AsTableDescriptor();

	public bool TryGetTable(string tableName, [MaybeNullWhen(false)] out Table table)
	{
		table = null;
		bool hasTable = tables.TryGetValue(tableName, out var internalTable);

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
			Table<TGameState, TSharedState, TPlayerState, TGameMove> table,
			Guid connectionId)
	{
		var connection = new Connection<
			TGameState,
			TSharedState,
			TPlayerState,
			TGameMove,
			TSerializedState,
			TSerializedMove>(table, connectionId, stateSerializer, moveDeserializer);

		return connection;
	}
}
