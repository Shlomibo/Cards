namespace GameEngine;

public sealed partial class Engine<TGameState, TSharedState, TPlayerState, TGameMove>
{
	#region Fields

	private readonly IState<TGameState, TSharedState, TPlayerState, TGameMove> state;
	#endregion

	#region Events

	public event EventHandler? Updated;
	#endregion

	#region Properties

	public IReadOnlyList<IPlayer<TSharedState, TPlayerState, TGameMove>> Players { get; }
	public TSharedState State => state.SharedState;
	#endregion

	#region Ctors

	public Engine(IState<TGameState, TSharedState, TPlayerState, TGameMove> state)
	{
		this.state = state ?? throw new ArgumentNullException(nameof(state));
		Players = Enumerable.Range(0, this.state.PlayersCount)
			.Select((player, id) => new Player(
				id,
				this,
				state.GetPlayerState(player)
			))
			.ToArray();
	}
	#endregion

	#region Methods

	public bool IsValidMove(TGameMove move, int? player = null)
	{
		return state.IsValidMove(move, player);
	}

	public void PlayMove(TGameMove move, int? playerId = null)
	{
		if (state.PlayMove(move, playerId))
		{
			Updated?.Invoke(this, EventArgs.Empty);
		}
	}
	#endregion
}
