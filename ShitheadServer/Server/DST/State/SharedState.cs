using static Shithead.State.ShitheadState;

namespace ShitheadServer.Server.DST.State
{
	public sealed class SharedState
	{
		private readonly SharedShitheadState state;

		public IReadOnlyList<SharedPlayerState> Players { get; }
		public IReadOnlyList<int> ActivePlayers => this.state.ActivePlayers;
		public object? LastMove => this.state.LastMove is (var move, var playerId)
			? new
			{
				playerId,
				move = ShitheadMove.FromGameMove(move).ToJsonObject(),
			}
			: null;
		public int DeckSize => this.state.DeckSize;
		public IReadOnlyList<object> DiscardPile { get; }
		public string GameState => this.state.GameState.ToString();
		public int CurrentTurnPlayer => this.state.CurrentTurnPlayer;

		public SharedState(SharedShitheadState state)
		{
			this.state = state;

			this.Players = state.Players
				.Select(player => new SharedPlayerState(player))
				.ToArray();
			this.DiscardPile = state.DiscardPile
				.Select(card => card.ToJsonObject())
				.ToArray();
		}
	}
}
