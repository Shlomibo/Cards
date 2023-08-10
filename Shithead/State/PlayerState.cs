using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead.State
{
	public sealed class PlayerState
	{
		public const int UNDERCARDS_COUNT = 3;

		public int Id { get; }
		public CardsDeck Hand { get; } = new CardsDeck();

		public Dictionary<int, CardFace<Card>> Undercards { get; }
		public Dictionary<int, Card> RevealedCards { get; } = new();

		public bool Won =>
			!this.Hand.Any() &&
			!this.RevealedCards.Any() &&
			!this.Undercards.Any();
		public bool RevealedCardsAccepted { get; set; } = false;

		public PlayerState(ICollection<Card> undercards, int id)
		{
			if (undercards.Count != UNDERCARDS_COUNT)
			{
				throw new ArgumentException($"Undercards count is {UNDERCARDS_COUNT}", nameof(undercards));
			}

			this.Id = id;
			this.Undercards = new Dictionary<int, CardFace<Card>>(
				undercards
				.Select((card, index) => new KeyValuePair<int, CardFace<Card>>(index, new CardFace<Card>(card))));
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
				return CanPlaceCardFromDictionary(this.RevealedCards);
			}
			else if (this.Undercards.Any())
			{
				if (cardIndices.Count() != 1)
				{
					return false;
				}

				int index = cardIndices.First();

				return index > 0 &&
					this.Undercards.ContainsKey(index) &&
					this.Undercards[index].IsRevealed;
			}
			else
			{
				return false;
			}

			bool CanPlaceCardFromList(IReadOnlyList<Card> cards) =>
				CanPlaceCardFromDictionary(new Dictionary<int, Card>(
					cards.Select((card, i) => new KeyValuePair<int, Card>(i, card))));

			bool CanPlaceCardFromDictionary(IReadOnlyDictionary<int, Card> cards)
			{
				if (cardIndices.Any(i => !cards.ContainsKey(i)))
				{
					return false;
				}

				var value = cards[cardIndices.First()].Value;

				return cardIndices.Skip(1).All(i => cards[i].Value == value);
			}
		}

		public bool CanRevealUndercard(int cardIndex) =>
			!this.Hand.Any() &&
			!this.RevealedCards.Any() &&
			this.Undercards.ContainsKey(cardIndex) &&
			this.Undercards.Values.All(card => !card.IsRevealed);

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
				this.RevealedCards.Remove(cardIndex);
			}
			else if (this.Undercards.ContainsKey(cardIndex) &&
				this.Undercards[cardIndex].IsRevealed == true)
			{
				this.Undercards.Remove(cardIndex);
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
				this.RevealedCards.Remove(GetJokerKey(this.RevealedCards, card => card.Value));
			}
			else if (this.Undercards.Any())
			{
				this.Undercards.Remove(GetJokerKey(
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

			int GetJokerIndex<T>(
				IEnumerable<T> list,
				Func<T, Value> cardValueSelection,
				Func<T, bool>? filter = null) =>
				GetJokerKey(
					list.Select((item, index) => new KeyValuePair<int, T>(index, item)),
					cardValueSelection,
					filter);

			int GetJokerKey<T>(
				IEnumerable<KeyValuePair<int, T>> list,
				Func<T, Value> cardValueSelection,
				Func<T, bool>? filter = null)
			{
				if (filter != null)
				{
					list = list.Where(item => filter(item.Value));
				}

				return list
					.Where(item => cardValueSelection(item.Value) == Value.Joker)
					.Select(item => (int?)item.Key)
					.FirstOrDefault() ?? -1;
			}
		}
	}
}
