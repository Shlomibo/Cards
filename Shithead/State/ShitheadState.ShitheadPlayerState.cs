using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead.State;

public sealed partial class ShitheadState
{
	public sealed class ShitheadPlayerState
	{
		private readonly ShitheadState state;

		private PlayerState PlayerState => state.players[PlayerId];
		private SharedPlayerState SharedPlayerState => state.SharedState.Players[PlayerId];

		public GameState GameState => state.GameState;
		public int PlayerId { get; }
		public IReadonlyDeck<Card> Hand { get; }
		public IReadOnlyDictionary<int, Card> RevealedCards => SharedPlayerState.RevealedCards;
		public IReadOnlyDictionary<int, Card?> Undercards => SharedPlayerState.Undercards;
		public bool Won => PlayerState.Won;
		public bool RevealedCardsAccepted => PlayerState.RevealedCardsAccepted;

		public ShitheadPlayerState(int playerId, ShitheadState state)
		{
			PlayerId = playerId;
			this.state = state;
			Hand = PlayerState.Hand.AsReadonly();
		}

		public Card GetCard(int cardIndex)
		{
			if (Hand.Count > 0)
			{
				return Hand[cardIndex];
			}
			else if (RevealedCards.Count > 0)
			{
				return RevealedCards[cardIndex];
			}
			else
			{
				return Undercards[cardIndex] ??
					throw new InvalidOperationException("The selected undercard is not revealed yet");
			}
		}
	}
}
