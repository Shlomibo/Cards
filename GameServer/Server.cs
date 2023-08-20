using GameEngine;
using GameServer.DST;

namespace GameServer
{
	public class Server<
		TInitOptions,
		TGameState,
		TSharedState,
		TPlayerState,
		TGameMove,
		TSerializedState,
		TSerializedMove>
		where TSerializedState : IStateUpdate<object, object>
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
			if (this.tables.ContainsKey(tableName))
			{
				throw new ArgumentException($"The table '{tableName}' already exists", nameof(tableName));
			}

			Table<TGameState, TSharedState, TPlayerState, TGameMove> table = new(tableName, tableMasterName);
			this.tables.Add(tableName, table);

			return CreateConnection(table, table.TableMaster.ConnectionId);
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

			if (!this.tables.TryGetValue(tableName, out var table))
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
				TSerializedMove>(table, connectionId, this.stateSerializer, this.moveDeserializer);

			return connection;
		}
	}
}