using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Deck.Cards.FrenchSuited;

/// <summary>
/// A deck of French-suited playing cards.
/// </summary>
public sealed record CardsDeck : IDeck<Card>
{
    private readonly CardsDeck<Card> _deck;

    /// <inheritdoc cref="CardsDeck{TCard}.CardsDeck()"/>
    public CardsDeck()
    {
        _deck = [];
    }

    /// <inheritdoc cref="CardsDeck{TCard}.CardsDeck(IEnumerable{TCard})"/>
    public CardsDeck(IEnumerable<Card> cards)
    {
        _deck = [.. cards];
    }

    #region Properties

    /// <inheritdoc/>
    public Card this[int index]
    {
        get => _deck[index];
        set => _deck[index] = value;
    }

    Card IReadOnlyList<Card>.this[int index] => _deck[index];

    /// <inheritdoc/>
    public Card? Top => _deck.Top;

    /// <inheritdoc/>
    public int Count => _deck.Count;

    bool ICollection<Card>.IsReadOnly => ((ICollection<Card>)_deck).IsReadOnly;
    #endregion

    #region Methods

    /// <summary>
    /// Returns a new full deck of cards.
    /// </summary>
    /// <param name="excludeJokers">
    /// <see langword="true"/> to include Jokers; otherwise, <see langword="false"/>.
    /// </param>
    public static CardsDeck FullDeck(bool excludeJokers = false) =>
        [.. Card.AllCards(excludeJokers)];

    /// <summary>
    /// Returns a new full shuffled deck of cards.
    /// </summary>
    /// <param name="excludeJokers">
    /// <see langword="true"/> to include Jokers; otherwise, <see langword="false"/>.
    /// </param>
    public static CardsDeck FullShuffledDeck(bool excludeJokers = false)
    {
        var deck = FullDeck(excludeJokers);
        deck.Shuffle();

        return deck;
    }

    #region IDeck<Card> Methods
    /// <inheritdoc/>
    public void Add(Card item) =>
        _deck.Add(item);

    /// <inheritdoc/>
    public void Add(params IEnumerable<Card> cards) =>
        _deck.Add(cards);

    /// <inheritdoc/>
    public void Clear() => _deck.Clear();

    /// <inheritdoc/>
    public bool Contains(Card item) => _deck.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(Card[] array, int arrayIndex) => _deck.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<Card> GetEnumerator() => _deck.GetEnumerator();

    /// <inheritdoc/>
    public int IndexOf(Card item) => _deck.IndexOf(item);

    /// <inheritdoc/>
    public void Insert(int index, Card item) => _deck.Insert(index, item);

    /// <inheritdoc/>
    public bool TryPop([MaybeNullWhen(false)] out Card card) => _deck.TryPop(out card);

    /// <inheritdoc/>
    public void Push(Card card) => _deck.Push(card);

    /// <inheritdoc/>
    public void Push(params IEnumerable<Card>? cards)
    {
        if (cards == null)
        {
            return;
        }

        foreach (var card in cards)
        {
            Push(card);
        }
    }

    /// <inheritdoc/>
    public bool Remove(Card item) => _deck.Remove(item);

    /// <inheritdoc/>
    public void RemoveAt(int index) => _deck.RemoveAt(index);

    /// <inheritdoc/>
    public void Shuffle() => _deck.Shuffle();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_deck).GetEnumerator();
    /// <inheritdoc/>
    public IReadonlyDeck<Card> AsReadonly() => new ReadonlyDeck<Card>(this);
    #endregion
    #endregion
}
