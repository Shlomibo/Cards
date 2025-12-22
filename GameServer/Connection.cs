using DTOs;

namespace GameServer;

/// <summary>
/// A connection of a player to the server.
/// </summary>
/// <typeparam name="TGameState">The type of the game's state.</typeparam>
/// <typeparam name="TSharedState">The type of the game's shared state.</typeparam>
/// <typeparam name="TPlayerState">The type of the game's player-specific state.</typeparam>
/// <typeparam name="TGameMove">The type of the game's moves.</typeparam>
/// <typeparam name="TSerializedState">The type of the game's serialized DTOs.</typeparam>
/// <typeparam name="TSerializedMove">The type of the game's serialized moves.</typeparam>
public class Connection<
    TGameState,
    TSharedState,
    TPlayerState,
    TGameMove,
    TSerializedState,
    TSerializedMove> : IDisposable
    where TSerializedState : State
{
    private bool _isDisposed;
    private readonly ITable<TGameState,
        TSharedState,
        TPlayerState,
        TGameMove> _table;
    internal Guid ConnectionId { get; }
    private readonly Func<TSharedState, TPlayerState, TSerializedState> _stateSerializer;
    private readonly Func<TSerializedMove, TGameMove> _moveDeserializer;
    private EventHandler<StateUpdatedEventArgs<TSerializedState>>? _stateUpdatedHandler;
    private StateUpdatedEventArgs<TSerializedState> _lastGameState;

    /// <summary>
    /// Occurs when the game's state is updated.
    /// </summary>
    public event EventHandler<StateUpdatedEventArgs<TSerializedState>>? StateUpdated
    {
        add
        {
            _stateUpdatedHandler += value;
            value?.Invoke(this, _lastGameState);
        }
        remove => _stateUpdatedHandler -= value;
    }

    private Table<TGameState,
        TSharedState,
        TPlayerState,
        TGameMove>.Player Player
    { get; }

    internal Connection(
        ITable<TGameState, TSharedState, TPlayerState, TGameMove> table,
        Guid connectionId,
        Func<TSharedState, TPlayerState, TSerializedState> stateSerializer,
        Func<TSerializedMove, TGameMove> moveDeserializer)
    {
        _table = table;
        ConnectionId = connectionId;
        _stateSerializer = stateSerializer;
        _moveDeserializer = moveDeserializer;

        Player = _table[ConnectionId];

        _lastGameState = new StateUpdatedEventArgs<TSerializedState>(
            new StateUpdate<TSerializedState>(
                _table.TableName,
                new CurrentPlayer(Player.Id, Player.Name, Player.ConnectionId),
                _table.AsTableDescriptor().Players
                    .Select(player => new Player(
                        player.Id,
                        player.Name,
                        PlayerState.Playing))));

        _table.GameUpdated += OnGameUpdated;
        _table.TableUpdated += OnTableUpdated;
    }

    /// <summary>
    /// Deserialize and pass the game move to the game server.
    /// </summary>
    /// <param name="move">The move to send.</param>
    public void PlayMove(TSerializedMove move)
    {
        ArgumentNullException.ThrowIfNull(move);

        _table.PlayMove(
            _moveDeserializer(move),
            Player.Id);
    }

    /// <summary>
    /// Close the connection and remove the player from the table.
    /// </summary>
    public void Close() => (this as IDisposable).Dispose();

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
                    new Player(player.Id, player.Name, PlayerState.Playing)),
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
                _table.RemovePlayer(ConnectionId);
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
}

/// <summary>
/// Event arguments for a state-updated event.
/// </summary>
/// <typeparam name="TSerializedState">The type of the game's state.</typeparam>
public sealed class StateUpdatedEventArgs<TSerializedState>
    where TSerializedState : State
{
    /// <summary>
    /// The updated game state.
    /// </summary>
    public StateUpdate<TSerializedState> State { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateUpdatedEventArgs{TSerializedState}"/> class.
    /// </summary>
    /// <param name="state"></param>
    public StateUpdatedEventArgs(StateUpdate<TSerializedState> state)
    {
        State = state;
    }
}
