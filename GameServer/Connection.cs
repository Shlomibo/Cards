using GameServer.DTO;

namespace GameServer;

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
			stateUpdatedHandler += value;
			value?.Invoke(this, lastGameState);
		}
		remove { stateUpdatedHandler -= value; }
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

		Player = this.table[this.connectionId];

		lastGameState = new StateUpdatedEventArgs<TSerializedState>(
			new StateUpdate<TSerializedState>(
				this.table.TableName,
				new CurrentPlayer(Player.Id, Player.Name, Player.ConnectionId),
				this.table.AsTableDescriptor().Players.Select(player => new DTO.Player(player.Id, player.Name))));

		this.table.GameUpdated += OnGameUpdated;
		this.table.TableUpdated += OnTableUpdated;
	}

	public void PlayMove(TSerializedMove move) => table.PlayMove(
		moveDeserializer(move),
		Player.Id);

	private void OnGameUpdated(
		object? sender,
		TableGameUpdateEventArgs<
			TGameState,
			TSharedState,
			TPlayerState,
			TGameMove> args)
	{
		OnStateUpdated(stateSerializer(args.SharedState, args.PlayersStates[Player.Id]));
	}

	private void OnTableUpdated(object? sender, EventArgs e) =>
		OnStateUpdated(lastGameState.State.GameState);

	private void OnStateUpdated(TSerializedState? state)
	{
		var gameStateUpdate = new StateUpdatedEventArgs<TSerializedState>(
			new StateUpdate<TSerializedState>(
				table.TableName,
				new CurrentPlayer(Player.Id, Player.Name, Player.ConnectionId),
				table.AsTableDescriptor().Players.Select(player =>
					new DTO.Player(player.Id, player.Name)),
				state));

		lastGameState = gameStateUpdate;

		stateUpdatedHandler?.Invoke(this, gameStateUpdate);
	}

	private void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			if (disposing)
			{
				table.GameUpdated -= OnGameUpdated;
				table.RemovePlayer(connectionId);

				Closed?.Invoke(this, EventArgs.Empty);
				Closed = null;
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
		State = state;
	}
}
