using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead;

public static class Consts
{
    public const int LastCardsCount = 3;
    public const int InitialPlayerCards = 6;
}

public static class DeckExtenstions
{
    public static Card? TopCardValue(this IReadonlyDeck<Card> deck) =>
        deck.Where(card => card.Value != Value.Three)
            .Select(card => (Card?)card)
            .FirstOrDefault();
}
