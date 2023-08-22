using GameEngine;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GameServer
{
	internal sealed class Table<
		TGameState,
		TSharedState,
		TPlayerState,
		TGameMove>
	{
		private readonly HashSet<string> playerNames = new();
		private readonly Dictionary<int, string> playerNamesByIds = new();
		private readonly Dictionary<int, Guid> playerConnectionIdsByIds = new();
		private readonly Dictionary<Guid, int> playerIdsByConnectionId = new();
		private Engine<TGameState, TSharedState, TPlayerState, TGameMove>? game;

		public event EventHandler? TableUpdated;
		public event EventHandler<TableGameUpdateEventArgs<
			TGameState,
			TSharedState,
			TPlayerState,
			TGameMove>>? GameUpdated;

		public Player TableMaster => this[0];

		public string TableName { get; }
		public Engine<TGameState, TSharedState, TPlayerState, TGameMove>? Game => this.game;

		[MemberNotNullWhen(true, nameof(Game))]
		public bool GameStarted => this.Game != null;

		public Player this[int playerId] => new(
			playerId,
			name: playerNamesByIds[playerId],
			connectionId: playerConnectionIdsByIds[playerId]);

		public Player this[Guid connectionId]
		{
			get
			{
				int playerId = this.playerIdsByConnectionId[connectionId];

				return new Player(
					playerId,
					name: playerNamesByIds[playerId],
					connectionId);
			}
		}

		public Table(string tableName, string tableMasterName)
		{
			if (string.IsNullOrEmpty(tableName))
			{
				throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
			}

			if (tableMasterName is null)
			{
				throw new ArgumentNullException(nameof(tableMasterName));
			}

			this.TableName = tableName;
			this.playerNames.Add(tableMasterName);
			AddPlayerWithId(0, tableMasterName);
		}

		public Player AddPlayer(string name)
		{
			if (!this.playerNames.Add(name))
			{
				throw new ArgumentException("A player with the same name already exists", nameof(name));
			}

			int id = 1 + playerConnectionIdsByIds.Keys.Max();

			return AddPlayerWithId(id, name);
		}

		public void RemovePlayer(int id)
		{
			if (this.playerConnectionIdsByIds.TryGetValue(id, out var connId))
			{
				this.playerIdsByConnectionId.Remove(connId);
			}

			this.playerConnectionIdsByIds.Remove(id);
			this.playerNamesByIds.Remove(id);

			OnTableUpdate();
		}

		private void OnTableUpdate()
		{
			this.TableUpdated?.Invoke(this, EventArgs.Empty);
		}

		public void RemovePlayer(Guid connectionId)
		{
			if (this.playerIdsByConnectionId.TryGetValue(connectionId, out int id))
			{
				RemovePlayer(id);
			}
		}

		public void RemovePlayer(Player player) =>
			RemovePlayer(player.Id);

		public void PlayMove(TGameMove move, int? playerId = null) =>
			this.game?.PlayMove(move, playerId);
		public void PlayMove(TGameMove move, Guid connectionId)
		{
			if (this.playerIdsByConnectionId.TryGetValue(connectionId, out int id))
			{
				this.game?.PlayMove(move, id);
			}
		}

		public void PlayMove(TGameMove move, Player player) => PlayMove(move, player.Id);

		public bool TrySetGame(Engine<TGameState, TSharedState, TPlayerState, TGameMove> game)
		{
			if (game != this.game && game == null)
			{
				return false;
			}

			try
			{
				SetGame(game);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void SetGame(Engine<TGameState, TSharedState, TPlayerState, TGameMove> game)
		{
			if (game != this.game)
			{
				if (this.game != null)
				{
					this.game.Updated -= OnGameUpdated;
				}

				this.game = game ?? throw new ArgumentNullException(nameof(game));
				this.game.Updated += OnGameUpdated;
			}

			void OnGameUpdated(object? _, EventArgs args) => this.GameUpdated?.Invoke(
					this,
					new TableGameUpdateEventArgs<TGameState, TSharedState, TPlayerState, TGameMove>(
						game.State,
						game.Players.Select(player => (this[player.PlayerId], player.State))));

			OnGameUpdated(this, EventArgs.Empty);
		}

		private Player AddPlayerWithId(int id, string name)
		{
			Guid connectionId = Guid.NewGuid();

			this.playerConnectionIdsByIds.Add(id, connectionId);
			this.playerIdsByConnectionId.Add(connectionId, id);
			this.playerNamesByIds.Add(id, name);

			OnTableUpdate();

			return this[id];
		}

		public Table AsTableDescriptor() =>
			new(
				this.TableMaster.AsDescriptor(),
				players: from kv in this.playerNamesByIds
						 let id = kv.Key
						 where id != this.TableMaster.Id
						 let name = kv.Value
						 select new Table.Player(id, name));

		public readonly struct Player : IEquatable<Player>
		{
			public static Player NoPlayer { get; } = new Player();
			public readonly int Id { get; }
			public readonly string Name { get; }
			public readonly Guid ConnectionId { get; }

			public Player(int id, string name, Guid connectionId)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
				}

				this.Id = id;
				this.Name = name;
				this.ConnectionId = connectionId;
			}

			public Table.Player AsDescriptor() =>
				new(this.Id, this.Name);

			public override string ToString() => $"({this.Id}): {this.Name}";

			// override object.Equals
			public override bool Equals(object? obj) =>
				obj is Player other && Equals(other);

			// override object.GetHashCode
			public override int GetHashCode() =>
				(this.Id, this.Name, this.ConnectionId).GetHashCode();

			public bool Equals(Player other) =>
				other.Id == this.Id &&
				other.Name == this.Name &&
				other.ConnectionId == this.ConnectionId;

			public static bool operator ==(Player left, Player right) => left.Equals(right);
			public static bool operator !=(Player left, Player right) => !(left == right);
		}
	}

	internal class TableGameUpdateEventArgs<
		TGameState,
		TSharedState,
		TPlayerState,
		TGameMove> : EventArgs
	{
		public TSharedState SharedState { get; }
		public PlayersStates<TGameState, TSharedState, TPlayerState, TGameMove> PlayersStates
		{ get; }

		public TableGameUpdateEventArgs(
			TSharedState sharedState,
			IEnumerable<(Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player, TPlayerState)> playersStates)
		{
			this.SharedState = sharedState;
			this.PlayersStates = new PlayersStates<TGameState, TSharedState, TPlayerState, TGameMove>(playersStates);
		}
	}

	internal class PlayersStates<TGameState, TSharedState, TPlayerState, TGameMove> :
		IReadOnlyDictionary<Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player, TPlayerState>
	{
		private readonly Dictionary<int, TPlayerState> playersStates;
		private readonly Dictionary<
			int,
			Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player> playersById;
		private readonly Dictionary<
			Guid,
			Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player> playersByConnectionId;

		public PlayersStates(
			IEnumerable<(
				Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player key,
				TPlayerState value)> playersStates)
		{
			this.playersStates = new Dictionary<int, TPlayerState>(
					playersStates.Select(kv => new KeyValuePair<int, TPlayerState>(kv.key.Id, kv.value)));
			this.playersById = new Dictionary<int, Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player>(
				playersStates.Select(kv => new KeyValuePair<
					int,
					Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player>(
						kv.key.Id, kv.key)));
			this.playersByConnectionId = new Dictionary<Guid, Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player>(
				playersStates.Select(kv => new KeyValuePair<
					Guid,
					Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player>(
						kv.key.ConnectionId, kv.key)));
		}

		public TPlayerState this[Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player player] =>
			this[player.Id];

		public TPlayerState this[int playerId] => this.playersStates[playerId];

		public TPlayerState this[Guid connectionId] => this[this.playersByConnectionId[connectionId]];

		public IEnumerable<Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player> Keys =>
			this.playersById.Values;

		public IEnumerable<TPlayerState> Values => this.playersStates.Values;

		public int Count => this.playersStates.Values.Count;

		public bool ContainsKey(Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player player) =>
			this.playersById.TryGetValue(player.Id, out var playerById) && player == playerById;

		public bool ContainsKey(int playerId) =>
			this.playersStates.ContainsKey(playerId);

		public bool ContainsKey(Guid connectionId) =>
			this.playersByConnectionId.ContainsKey(connectionId);

		public IEnumerator<KeyValuePair<
			Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player,
			TPlayerState>> GetEnumerator()
		{
			foreach (var kv in this.playersStates)
			{
				var player = this.playersById[kv.Key];
				yield return new KeyValuePair<
					Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player,
					TPlayerState>(player, kv.Value);
			}
		}

		public bool TryGetValue(
			Table<TGameState, TSharedState, TPlayerState, TGameMove>.Player key,
			[MaybeNullWhen(false)] out TPlayerState value)
		{
			value = default;
			bool hasKey = ContainsKey(key);

			if (hasKey)
			{
				value = this[key];
			}

			return hasKey;
		}

		public bool TryGetValue(int playerId, [MaybeNullWhen(false)] out TPlayerState value) =>
			this.playersStates.TryGetValue(playerId, out value);

		public bool TryGetValue(Guid connectionId, [MaybeNullWhen(false)] out TPlayerState value)
		{
			value = default;
			bool hasKey = ContainsKey(connectionId);

			if (hasKey)
			{
				value = this[connectionId];
			}

			return hasKey;
		}

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();
	}
}
