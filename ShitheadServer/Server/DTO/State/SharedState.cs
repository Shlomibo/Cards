using static Shithead.State.ShitheadState;

namespace ShitheadServer.Server.DTO.State;

public sealed class SharedState
{
	private readonly SharedShitheadState state;

	public IReadOnlyList<SharedPlayerState> Players { get; }
	public IReadOnlyList<int> ActivePlayers => state.ActivePlayers;
	public object? LastMove => state.LastMove is (var move, var playerId)
		? new
		{
			playerId,
			move = ShitheadMove.FromGameMove(move).ToJsonObject(),
		}
		: null;
	public int DeckSize => state.DeckSize;
	public IReadOnlyList<object> DiscardPile { get; }
	public string GameState => state.GameState.ToString();
	public int CurrentTurnPlayer => state.CurrentTurnPlayer;

	public SharedState(SharedShitheadState state)
	{
		this.state = state;

		Players = state.Players
			.Select(player => new SharedPlayerState(player))
			.ToArray();
		DiscardPile = state.DiscardPile
			.Select(card => card.ToJsonObject())
			.ToArray();
	}
}
