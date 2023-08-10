using Deck;
using Deck.Cards.FrenchSuited;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shithead.State
{
	internal sealed class PlayerState
	{
		public const int UNDERCARDS_COUNT = 3;

		public CardsDeck Hand { get; } = new CardsDeck();

		public List<CardFace<Card>> Undercards { get; }
		public List<Card> RevealedCards { get; } = new();

		public bool Won { get; set; } = false;
		public bool RevealedCardsAccepted { get; set; } = false;

		public PlayerState(ICollection<Card> undercards)
		{
			if (undercards.Count != UNDERCARDS_COUNT)
			{
				throw new ArgumentException($"Undercards count is {UNDERCARDS_COUNT}", nameof(undercards));
			}

			this.Undercards = undercards
				.Select(card => new CardFace<Card>(card))
				.ToList();
		}

		public bool CanSetRevealedCard(int cardIndex) =>
			!this.RevealedCardsAccepted &&
			this.RevealedCards.Count < UNDERCARDS_COUNT &&
			cardIndex >= 0 &&
			cardIndex < this.Hand.Count;

		public bool CanUnsetRevealedCard(int cardIndex) =>
			!this.RevealedCardsAccepted &&
			this.RevealedCards.Count > 0 &&
			cardIndex >= 0 &&
			cardIndex < this.RevealedCards.Count;

		public bool CanAcceptSelectedRevealedCards() =>
			!this.RevealedCardsAccepted &&
			this.RevealedCards.Count == UNDERCARDS_COUNT;

		public bool CanReselectRevealedCards() => this.RevealedCardsAccepted;

		public bool CanPlaceJoker() => this.Hand.Any(card => card.Value == Value.Joker);

		public bool CanPlaceCard(IEnumerable<int> cardIndices)
		{
			if (!cardIndices.Any() || cardIndices.Any(i => i < 0 || i >= this.Hand.Count))
			{
				return false;
			}

			var value = this.Hand[cardIndices.First()].Value;

			return cardIndices.Skip(1).All(i => this.Hand[i].Value == value);
		}
	}
}
