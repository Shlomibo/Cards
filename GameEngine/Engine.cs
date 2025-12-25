namespace GameEngine;

/// <inheritdoc cref="IEngine{TSharedState, TPlayerState, TGameMove}"/>
public partial class Engine<TGameState, TSharedState, TPlayerState, TGameMove> : IEngine<TSharedState, TPlayerState, TGameMove>
{

    private readonly IState<TGameState, TSharedState, TPlayerState, TGameMove> _state;

    /// <inheritdoc/>
    public event EventHandler? Updated;

    /// <inheritdoc/>
    public IReadOnlyList<IPlayer<TSharedState, TPlayerState, TGameMove>> Players { get; }

    /// <inheritdoc/>
    public TSharedState State => _state.SharedState;

    /// <summary>
    /// Initializes a new instance of the <see cref="Engine{TGameState, TSharedState, TPlayerState, TGameMove}"/> class.
    /// </summary>
    /// <param name="state">The initial state the game begins with.</param>
    public Engine(IState<TGameState, TSharedState, TPlayerState, TGameMove> state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        Players = [.. Enumerable.Range(0, _state.PlayersCount)
            .Select((player, id) => new Player(
                id,
                this,
                state.GetPlayerState(player)
            ))];
    }

    /// <inheritdoc/>
    public bool IsValidMove(TGameMove move, int? player = null) => _state.IsValidMove(move, player);

    /// <inheritdoc/>
    public void PlayMove(TGameMove move, int? playerId = null)
    {
        if (_state.PlayMove(move, playerId))
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
