using Deck;
using Deck.Cards.FrenchSuited;

namespace Shithead;

/// <summary>
/// Holds constant values for the Shithead game.
/// </summary>
public static class Consts
{
    /// <summary>
    /// The number of cards which remain on the table for the end of the game.
    /// </summary>
    public const int LastCardsCount = 3;

    /// <summary>
    /// The number of initial cards dealt to each player.
    /// </summary>
    public const int InitialPlayerCards = 6;
}

/// <summary>
/// Provides extension methods for <see cref="IReadonlyDeck{T}"/>.
/// </summary>
public static class DeckExtenstions
{
    /// <summary>
    /// Gets the value of the top card in the deck.
    /// </summary>
    /// <remarks>
    /// As <see cref="Value.Three"/> cards are invisible, they are ignored when getting the top card value.
    /// </remarks>
    /// <param name="deck">The deck to get top value from.</param>
    /// <returns>The top card which does not have a value of <see cref="Value.Three"/>.</returns>
    public static Card? TopCardValue(this IReadonlyDeck<Card> deck) =>
        deck.Where(card => card.Value != Value.Three)
            .Select(card => (Card?)card)
            .FirstOrDefault();
}
