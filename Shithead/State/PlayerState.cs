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
			if (!cardIndices.Any())
			{
				return false;
			}

			if (this.Hand.Any())
			{
				return CanPlaceCardFromList(this.Hand);
			}
			else if (this.RevealedCards.Any())
			{
				return CanPlaceCardFromList(this.RevealedCards);
			}
			else if (this.Undercards.Any())
			{
				if (cardIndices.Count() != 1)
				{
					return false;
				}

				int index = cardIndices.First();

				return index > 0 &&
					index < this.Undercards.Count &&
					this.Undercards[index].IsRevealed;
			}
			else
			{
				return false;
			}

			bool CanPlaceCardFromList(IList<Card> cards)
			{
				if (cardIndices.Any(i => i < 0 || i >= cards.Count))
				{
					return false;
				}

				var value = cards[cardIndices.First()].Value;

				return cardIndices.Skip(1).All(i => cards[i].Value == value);
			}
		}

		public Card GetCard(int cardIndex)
		{
			if (this.Hand.Any())
			{
				return this.Hand[cardIndex];
			}
			else if (this.RevealedCards.Any())
			{
				return this.RevealedCards[cardIndex];
			}
			else if (this.Undercards.Any() && this.Undercards[cardIndex].IsRevealed)
			{
				return this.Undercards[cardIndex].Card;
			}
			else
			{
				throw new InvalidOperationException($"Cannot get card from index {cardIndex}");
			}
		}

		public void RemoveCard(int cardIndex)
		{
			if (this.Hand.Any())
			{
				this.Hand.RemoveAt(cardIndex);
			}
			else if (this.RevealedCards.Any())
			{
				this.RevealedCards.RemoveAt(cardIndex);
			}
			else if (this.Undercards.ElementAtOrDefault(cardIndex)?.IsRevealed == true)
			{
				this.Undercards.RemoveAt(cardIndex);
			}
		}

		public void RemoveJoker()
		{
			if (this.Hand.Any())
			{
				RemoveFromList(this.Hand, GetJokerIndex(this.Hand, card => card.Value));
			}
			else if (this.RevealedCards.Any())
			{
				RemoveFromList(this.RevealedCards, GetJokerIndex(this.RevealedCards, card => card.Value));
			}
			else if (this.Undercards.Any())
			{
				RemoveFromList(this.Undercards, GetJokerIndex(
					this.Undercards,
					cardface => cardface.Card.Value,
					cardface => cardface.IsRevealed));
			}

			void RemoveFromList<T>(IList<T> list, int index)
			{
				if (index != -1)
				{
					list.RemoveAt(index);
				}
			}

			int GetJokerIndex<T>(IEnumerable<T> list, Func<T, Value> cardValueSelection, Func<T, bool>? filter = null)
			{
				var indexedList = list
					.Select((item, index) => (value: item, index));

				if (filter != null)
				{
					indexedList = indexedList.Where(item => filter(item.value));
				}

				return indexedList
					.Where(item => cardValueSelection(item.value) == Value.Joker)
					.Select(item => (int?)item.index)
					.FirstOrDefault() ?? -1;
			}
		}
	}
}
