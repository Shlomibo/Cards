using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead.State;

public sealed class PlayerState
{
	public const int UNDERCARDS_COUNT = 3;

	public int Id { get; }
	public CardsDeck Hand { get; } = new CardsDeck();

	public Dictionary<int, CardFace<Card>> Undercards { get; }
	public bool DidLeaveGame { get; set; } = false;
	public Dictionary<int, Card> RevealedCards { get; } = new();

	public bool Won =>
		!DidLeaveGame &&
		!Hand.Any() &&
		!RevealedCards.Any() &&
		!Undercards.Any();
	public bool RevealedCardsAccepted { get; set; } = false;

	public PlayerState(ICollection<Card> undercards, int id)
	{
		if (undercards.Count != UNDERCARDS_COUNT)
		{
			throw new ArgumentException($"Undercards count is {UNDERCARDS_COUNT}", nameof(undercards));
		}

		Id = id;
		Undercards = new Dictionary<int, CardFace<Card>>(
			undercards
			.Select((card, index) => new KeyValuePair<int, CardFace<Card>>(index, new CardFace<Card>(card))));
	}

	public bool CanSetRevealedCard(int cardIndex, int target) =>
		!RevealedCardsAccepted &&
		RevealedCards.Count < UNDERCARDS_COUNT &&
		cardIndex >= 0 &&
		cardIndex < Hand.Count &&
		!RevealedCards.ContainsKey(target);

	public bool CanUnsetRevealedCard(int cardIndex) =>
		!RevealedCardsAccepted &&
		RevealedCards.Count > 0 &&
		cardIndex >= 0 &&
		RevealedCards.ContainsKey(cardIndex);

	public bool CanAcceptSelectedRevealedCards() =>
		!RevealedCardsAccepted &&
		RevealedCards.Count == UNDERCARDS_COUNT;

	public bool CanReselectRevealedCards() => RevealedCardsAccepted;

	public bool CanPlaceJoker()
	{
		if (Hand.Count > 0)
		{
			return Hand.Any(IsJoker);
		}
		else if (RevealedCards.Count > 0)
		{
			return RevealedCards.Values.Any(IsJoker);
		}
		else
		{
			return Undercards.Values
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

		return CanPlaceCardFromList(Hand);

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
		!Hand.Any() &&
		!RevealedCards.Any() &&
		Undercards.ContainsKey(cardIndex) &&
		Undercards.Values.All(card => !card.IsRevealed);

	public bool CanTakeUndercards(int[] cardIndices)
	{
		if (cardIndices.Length == 0 || Hand.Count > 0)
		{
			return false;
		}
		else if (RevealedCards.Count > 0)
		{
			if (!cardIndices.All(i => RevealedCards.ContainsKey(i)))
			{
				return false;
			}

			var value = RevealedCards[cardIndices[0]].Value;

			return cardIndices.Skip(1).All(i => RevealedCards[i].Value == value);
		}
		else
		{
			return cardIndices.Length == 1 &&
				Undercards[cardIndices[0]].IsRevealed;
		}
	}

	public Card GetCard(int cardIndex)
	{
		return Hand[cardIndex];
	}

	public void RemoveCard(int cardIndex)
	{
		Hand.RemoveAt(cardIndex);
	}

	public void RemoveJoker()
	{
		RemoveFromList(Hand, GetJokerIndex(Hand, card => card.Value));

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
