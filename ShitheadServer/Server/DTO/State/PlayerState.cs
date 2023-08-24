using static Shithead.State.ShitheadState;

namespace ShitheadServer.Server.DTO.State
{
	public sealed class PlayerState
	{
		private readonly ShitheadPlayerState state;

		public string GameState => this.state.GameState.ToString();
		public int PlayerId => this.state.PlayerId;
		public object[] Hand { get; }
		public IReadOnlyDictionary<int, object> RevealedCards { get; }
		public IReadOnlyDictionary<int, object?> Undercards { get; }
		public bool Won => this.state.Won;
		public bool RevealedCardsAccepted => this.state.RevealedCardsAccepted;

		public PlayerState(ShitheadPlayerState state)
		{
			this.state = state;

			this.Hand = state.Hand
				.Select(card => card.ToJsonObject())
				.ToArray();
			this.RevealedCards = new Dictionary<int, object>(
				state.RevealedCards
					.Select(kv => new KeyValuePair<int, object>(
						kv.Key,
						kv.Value.ToJsonObject())));
			this.Undercards = new Dictionary<int, object?>(
				state.Undercards
					.Select(kv => new KeyValuePair<int, object?>(
						kv.Key,
						kv.Value?.ToJsonObject())));
		}
	}
}
