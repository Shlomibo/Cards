using Shithead.State;

namespace ShitheadServer.Server.DTO.State
{
	public sealed class SharedPlayerState
	{
		private readonly ShitheadState.SharedPlayerState state;

		public int Id => this.state.Id;
		public bool Won => this.state.Won;
		public int CardsCount => this.state.CardsCount;
		public bool RevealedCardsAccepted => this.state.RevealedCardsAccepted;
		public IReadOnlyDictionary<int, object> RevealedCards { get; }
		public IReadOnlyDictionary<int, object?> Undercards { get; }

		public SharedPlayerState(ShitheadState.SharedPlayerState state)
		{
			this.state = state;

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
