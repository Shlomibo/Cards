using Shithead.State;
using static Shithead.State.ShitheadState;
using System.Text.Json.Serialization;

namespace ShitheadServer.Server.DST.State
{
	public sealed class PlayerState
	{
		private readonly ShitheadPlayerState state;

		public GameState GameState => this.state.GameState;
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
