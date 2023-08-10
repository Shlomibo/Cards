using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead.State
{
	public sealed partial class ShitheadState
	{
		public sealed class ShitheadPlayerState
		{
			private readonly ShitheadState state;

			private PlayerState PlayerState => this.state.players[this.PlayerId];
			private SharedPlayerState SharedPlayerState => this.state.SharedState.Players[this.PlayerId];

			public GameState GameState => this.state.GameState;
			public int PlayerId { get; }
			public IReadonlyDeck<Card> Hand { get; }
			public IReadOnlyDictionary<int, Card> RevealedCards => this.SharedPlayerState.RevealedCards;
			public IReadOnlyDictionary<int, Card?> Undercards => this.SharedPlayerState.Undercards;


			public ShitheadPlayerState(int playerId, ShitheadState state)
			{
				this.PlayerId = playerId;
				this.state = state;
				this.Hand = this.PlayerState.Hand.AsReadonly();
			}
		}
	}
}
