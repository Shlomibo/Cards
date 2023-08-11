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

		public bool CanSetRevealedCard(int cardIndex, int target) =>
			!this.RevealedCardsAccepted &&
			this.RevealedCards.Count < UNDERCARDS_COUNT &&
			cardIndex >= 0 &&
			cardIndex < this.Hand.Count &&
			!this.RevealedCards.ContainsKey(target);

		public bool CanUnsetRevealedCard(int cardIndex) =>
			!this.RevealedCardsAccepted &&
			this.RevealedCards.Count > 0 &&
			cardIndex >= 0 &&
			this.RevealedCards.ContainsKey(cardIndex);

		public bool CanAcceptSelectedRevealedCards() =>
			!this.RevealedCardsAccepted &&
			this.RevealedCards.Count == UNDERCARDS_COUNT;

		public bool CanReselectRevealedCards() => this.RevealedCardsAccepted;

		public bool CanPlaceJoker()
		{
			if (this.Hand.Count > 0)
			{
				return this.Hand.Any(IsJoker);
			}
			else if (this.RevealedCards.Count > 0)
			{
				return this.RevealedCards.Values.Any(IsJoker);
			}
			else
			{
				return this.Undercards.Values
					.Where(undercard => undercard.IsRevealed)
					.Select(undercard => undercard.Card)
					.Any(IsJoker);
			}

			static bool IsJoker(Card card) => card.Value == Value.Joker;
		}

		public bool CanPlaceCard(IEnumerable<int> cardIndices)
		{
			if (!cardIndices.Any())
			{
				return false;
			}

			return CanPlaceCardFromList(this.Hand);

			bool CanPlaceCardFromList(IReadOnlyList<Card> cards)
			{
				if (cardIndices.Any(i => i < 0 || i >= cards.Count))
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

		public bool CanTakeUndercards(int[] cardIndices)
		{
			if (cardIndices.Length == 0 || this.Hand.Count > 0)
			{
				return false;
			}
			else if (this.RevealedCards.Count > 0)
			{
				if (!cardIndices.All(i => this.RevealedCards.ContainsKey(i)))
				{
					return false;
				}

				var value = this.RevealedCards[cardIndices[0]].Value;

				return cardIndices.Skip(1).All(i => this.RevealedCards[i].Value == value);
			}
			else
			{
				return cardIndices.Length == 1 &&
					this.Undercards[cardIndices[0]].IsRevealed;
			}
		}

		public Card GetCard(int cardIndex)
		{
			return this.Hand[cardIndex];
		}


		public void RemoveCard(int cardIndex)
		{
			this.Hand.RemoveAt(cardIndex);
		}

		public void RemoveJoker()
		{
			RemoveFromList(this.Hand, GetJokerIndex(this.Hand, card => card.Value));

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
