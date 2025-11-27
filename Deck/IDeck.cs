using System.Diagnostics.CodeAnalysis;

namespace Deck;

/// <summary>
/// A read-only view of a deck of cards.
/// </summary>
/// <typeparam name="TCard">The type of cards in the deck.</typeparam>
public interface IReadonlyDeck<TCard> : IReadOnlyList<TCard>
    where TCard : struct
{
    /// <summary>
    /// Gets the top card of the deck if there are any cards in it.
    /// </summary>
	TCard? Top { get; }
}

/// <summary>
/// A deck of cards.
/// </summary>
/// <typeparam name="TCard">The type of cards in the deck.</typeparam>
public interface IDeck<TCard> : IList<TCard>, IReadonlyDeck<TCard>
    where TCard : struct
{
    /// <summary>
    /// Gets the number of cards in the deck.
    /// </summary>
	new int Count { get; }

    /// <summary>
    /// Gets or sets the card at the specified index.
    /// </summary>
	new TCard this[int index] { get; set; }

    /// <summary>
    /// Pushes a card or multiple cards on top of the deck.
    /// </summary>
    /// <param name="card">The card to push.</param>
	void Push(TCard card);

    /// <summary>
    /// Pushes multiple cards on top of the deck.
    /// </summary>
    /// <param name="cards">The cards to push.</param>
	void Push(params IEnumerable<TCard> cards);

    /// <summary>
    /// Adds multiple cards to the bottom of the deck.
    /// </summary>
    /// <param name="cards">The cards to add.</param>
	void Add(params IEnumerable<TCard> cards);

    /// <summary>
    /// Pops the top card from the deck.
    /// </summary>
    /// <returns>The top card.</returns>
	TCard Pop() =>
        TryPop(out TCard card)
            ? card
            : throw new InvalidOperationException("The deck is empty.");

    /// <summary>
    /// Tries to pop the top card from the deck.
    /// </summary>
    /// <param name="card">Contains the poped card if there was any card to pop.</param>
    /// <returns>
    /// <see langword="true"/> if a card was poped successfully.<br/>
    /// Otherwise, <see langword="false"/>.
    /// </returns>
    bool TryPop([MaybeNullWhen(false)] out TCard card);

    /// <summary>
    /// Shuffles the deck.
    /// </summary>
	void Shuffle();

    /// <summary>
    /// Gets a read-only view of the deck.
    /// </summary>
    /// <returns>A read-only view of the deck.</returns>
	IReadonlyDeck<TCard> AsReadonly();
}
