namespace GameEngine;

/// <summary>
/// A generic game engine.
/// </summary>
/// <typeparam name="TGameState">The type of the internal state of the game.</typeparam>
/// <typeparam name="TSharedState">The type of the state that is visible to all players.</typeparam>
/// <typeparam name="TPlayerState">The type of the state that each player privately has.</typeparam>
/// <typeparam name="TGameMove">The type of available game moves.</typeparam>
public partial class Engine<TGameState, TSharedState, TPlayerState, TGameMove>
{

    private readonly IState<TGameState, TSharedState, TPlayerState, TGameMove> _state;

    /// <summary>
    /// Occurs when the game state is updated.
    /// </summary>
    public event EventHandler? Updated;

    /// <summary>
    /// Gets the players in the game.
    /// </summary>
    public IReadOnlyList<IPlayer<TSharedState, TPlayerState, TGameMove>> Players { get; }

    /// <summary>
    /// Gets the shared state that is visible to all players.
    /// </summary>
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

    /// <inheritdoc cref="IState{TGameState, TSharedState, TPlayerState, TGameMove}.IsValidMove(TGameMove, int?)"/>
    public bool IsValidMove(TGameMove move, int? player = null) => _state.IsValidMove(move, player);

    /// <inheritdoc cref="IState{TGameState, TSharedState, TPlayerState, TGameMove}.PlayMove(TGameMove, int?)"/>
    public void PlayMove(TGameMove move, int? playerId = null)
    {
        if (_state.PlayMove(move, playerId))
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
