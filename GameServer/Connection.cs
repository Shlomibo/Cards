using GameServer.DTO;

namespace GameServer;

public sealed class Connection<
    TGameState,
    TSharedState,
    TPlayerState,
    TGameMove,
    TSerializedState,
    TSerializedMove> : IDisposable
    where TSerializedState : State
{
    private bool _isDisposed;
    private readonly Table<TGameState,
        TSharedState,
        TPlayerState,
        TGameMove> _table;
    private readonly Guid _connectionId;
    private readonly Func<TSharedState, TPlayerState, TSerializedState> _stateSerializer;
    private readonly Func<TSerializedMove, TGameMove> _moveDeserializer;
    private EventHandler<StateUpdatedEventArgs<TSerializedState>>? _stateUpdatedHandler;
    private StateUpdatedEventArgs<TSerializedState> _lastGameState;

    public event EventHandler<StateUpdatedEventArgs<TSerializedState>>? StateUpdated
    {
        add
        {
            _stateUpdatedHandler += value;
            value?.Invoke(this, _lastGameState);
        }
        remove => _stateUpdatedHandler -= value;
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
        _table = table;
        _connectionId = connectionId;
        _stateSerializer = stateSerializer;
        _moveDeserializer = moveDeserializer;

        Player = _table[_connectionId];

        _lastGameState = new StateUpdatedEventArgs<TSerializedState>(
            new StateUpdate<TSerializedState>(
                _table.TableName,
                new CurrentPlayer(Player.Id, Player.Name, Player.ConnectionId),
                _table.AsTableDescriptor().Players.Select(player => new DTO.Player(player.Id, player.Name))));

        _table.GameUpdated += OnGameUpdated;
        _table.TableUpdated += OnTableUpdated;
    }

    public void PlayMove(TSerializedMove move) => _table.PlayMove(
        _moveDeserializer(move),
        Player.Id);

    private void OnGameUpdated(
        object? sender,
        TableGameUpdateEventArgs<
            TGameState,
            TSharedState,
            TPlayerState,
            TGameMove> args)
        =>
        OnStateUpdated(_stateSerializer(args.SharedState, args.PlayersStates[Player.Id]));

    private void OnTableUpdated(object? sender, EventArgs e) =>
        OnStateUpdated((TSerializedState)(object)_lastGameState.State);

    private void OnStateUpdated(TSerializedState? state)
    {
        StateUpdatedEventArgs<TSerializedState> gameStateUpdate = new(
            new StateUpdate<TSerializedState>(
                _table.TableName,
                new CurrentPlayer(Player.Id, Player.Name, Player.ConnectionId),
                _table.AsTableDescriptor().Players.Select(player =>
                    new Player(player.Id, player.Name)),
                state));

        _lastGameState = gameStateUpdate;

        _stateUpdatedHandler?.Invoke(this, gameStateUpdate);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _table.GameUpdated -= OnGameUpdated;
                _table.RemovePlayer(_connectionId);

                Closed?.Invoke(this, EventArgs.Empty);
                Closed = null;
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _isDisposed = true;
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

public sealed class StateUpdatedEventArgs<TSerializedState>
    where TSerializedState : State
{
    public StateUpdate<TSerializedState> State { get; }

    public StateUpdatedEventArgs(StateUpdate<TSerializedState> state)
    {
        State = state;
    }
}
