﻿namespace GameServer.DST
{
	public sealed class StateUpdate<TState>
		where TState : class, IState<object, object>
	{
		public string TableName { get; }
		public CurrentPlayer CurrentPlayer { get; }
		public IReadOnlyDictionary<int, Player> Table { get; }
		public TState? GameState { get; }

		public StateUpdate(
			string tableName,
			CurrentPlayer currentPlayer,
			IReadOnlyDictionary<int, Player> table,
			TState? state = null)
		{
			this.TableName = tableName;
			this.CurrentPlayer = currentPlayer;
			this.Table = table;
			this.GameState = state;
		}

		public StateUpdate(
			string tableName,
			CurrentPlayer currentPlayer,
			IEnumerable<Player> players,
			TState? state = null)
			: this(tableName, currentPlayer, players.ToDictionary(player => player.PlayerId), state)
		{
		}
	}

	public sealed record Player(int PlayerId, string PlayerName) { }
	public sealed record CurrentPlayer(int PlayerId, string PlayerName, Guid ConnectionId) { }
}
