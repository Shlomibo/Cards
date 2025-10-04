using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead;

public static class Consts
{
	public const int LAST_CARDS_COUNT = 3;
	public const int INITIAL_PLAYER_CARDS = 6;
}

public static class DeckExtenstions
{
	public static Card? TopCardValue(this IReadonlyDeck<Card> deck) =>
		deck.Where(card => card.Value != Value.Three)
			.Select(card => (Card?)card)
			.FirstOrDefault();
}
