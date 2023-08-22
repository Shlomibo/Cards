using GameServer.DST;

namespace GameServer
{
	public sealed class Connection<
		TGameState,
		TSharedState,
		TPlayerState,
		TGameMove,
		TSerializedState,
		TSerializedMove> : IDisposable
		where TSerializedState : class, IState<object, object>
	{
		private bool isDisposed;
		private readonly Table<TGameState,
			TSharedState,
			TPlayerState,
			TGameMove> table;
		private readonly Guid connectionId;
		private readonly Func<TSharedState, TPlayerState, TSerializedState> stateSerializer;
		private readonly Func<TSerializedMove, TGameMove> moveDeserializer;
		private EventHandler<StateUpdatedEventArgs<TSerializedState>>? stateUpdatedHandler;
		private StateUpdatedEventArgs<TSerializedState> lastGameState;

		public event EventHandler<StateUpdatedEventArgs<TSerializedState>>? StateUpdated
		{
			add
			{
				this.stateUpdatedHandler += value;
				value?.Invoke(this, this.lastGameState);
			}
			remove { this.stateUpdatedHandler -= value; }
		}
		public event EventHandler? Closed;

		private Table<TGameState,
			TSharedState,
			TPlayerState,
			TGameMove>.Player Player
		{ get; }

		internal Connection(
			Table<TGameState, TSharedState, TPlayerState, TGameMove> table,
			Guid connectionId,
			Func<TSharedState, TPlayerState, TSerializedState> stateSerializer,
			Func<TSerializedMove, TGameMove> moveDeserializer)
		{
			this.table = table;
			this.connectionId = connectionId;
			this.stateSerializer = stateSerializer;
			this.moveDeserializer = moveDeserializer;

			this.Player = this.table[this.connectionId];

			this.lastGameState = new StateUpdatedEventArgs<TSerializedState>(
				new StateUpdate<TSerializedState>(
					this.table.TableName,
					new CurrentPlayer(this.Player.Id, this.Player.Name, this.Player.ConnectionId),
					this.table.AsTableDescriptor().Players.Select(player => new DST.Player(player.Id, player.Name))));

			this.table.GameUpdated += OnGameUpdated;
			this.table.TableUpdated += OnTableUpdated;
		}

		public void PlayMove(TSerializedMove move) => this.table.PlayMove(
			this.moveDeserializer(move),
			this.Player.Id);

		private void OnGameUpdated(
			object? sender,
			TableGameUpdateEventArgs<
				TGameState,
				TSharedState,
				TPlayerState,
				TGameMove> args)
		{
			OnStateUpdated(this.stateSerializer(args.SharedState, args.PlayersStates[this.Player.Id]));
		}

		private void OnTableUpdated(object? sender, EventArgs e) =>
			OnStateUpdated(this.lastGameState.State.GameState);

		private void OnStateUpdated(TSerializedState? state)
		{
			var gameStateUpdate = new StateUpdatedEventArgs<TSerializedState>(
				new StateUpdate<TSerializedState>(
					this.table.TableName,
					new CurrentPlayer(this.Player.Id, this.Player.Name, this.Player.ConnectionId),
					this.table.AsTableDescriptor().Players.Select(player =>
						new DST.Player(player.Id, player.Name)),
					state));

			this.lastGameState = gameStateUpdate;

			this.stateUpdatedHandler?.Invoke(this, gameStateUpdate);
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					this.table.GameUpdated -= OnGameUpdated;
					this.table.RemovePlayer(this.connectionId);

					this.Closed?.Invoke(this, EventArgs.Empty);
					this.Closed = null;
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				isDisposed = true;
			}
		}

		void IDisposable.Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void Close() => (this as IDisposable).Dispose();
	}

	public sealed class StateUpdatedEventArgs<TSerializedState> : EventArgs
		where TSerializedState : class, IState<object, object>
	{
		public StateUpdate<TSerializedState> State { get; }

		public StateUpdatedEventArgs(StateUpdate<TSerializedState> state)
		{
			this.State = state;
		}
	}
}
