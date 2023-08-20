namespace GameServer
{
	public sealed class Connection<
		TGameState,
		TSharedState,
		TPlayerState,
		TGameMove,
		TSerializedState,
		TSerializedMove> : IDisposable
	{
		private bool isDisposed;
		private readonly Table<TGameState,
			TSharedState,
			TPlayerState,
			TGameMove> table;
		private readonly Guid connectionId;
		private readonly Func<TSharedState, TPlayerState, TSerializedState> stateSerializer;
		private readonly Func<TSerializedMove, TGameMove> moveDeserializer;

		public event EventHandler<StateUpdatedEventArgs<TSerializedState>>? StateUpdated;
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

			this.table.GameUpdated += OnGameUpdated;
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
			this.StateUpdated?.Invoke(this, new StateUpdatedEventArgs<TSerializedState>(
				this.stateSerializer(args.SharedState, args.PlayersStates[this.Player.Id])));
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
	{
		public TSerializedState State { get; }

		public StateUpdatedEventArgs(TSerializedState state)
		{
			this.State = state;
		}
	}
}
