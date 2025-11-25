namespace GameEngine;

public sealed partial class Engine<TGameState, TSharedState, TPlayerState, TGameMove>
{

    private readonly IState<TGameState, TSharedState, TPlayerState, TGameMove> _state;


    public event EventHandler? Updated;

    public IReadOnlyList<IPlayer<TSharedState, TPlayerState, TGameMove>> Players { get; }
    public TSharedState State => _state.SharedState;


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


    public bool IsValidMove(TGameMove move, int? player = null) => _state.IsValidMove(move, player);

    public void PlayMove(TGameMove move, int? playerId = null)
    {
        if (_state.PlayMove(move, playerId))
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
