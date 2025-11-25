using System.Collections;

namespace Deck.Cards.FrenchSuited;

public sealed class CardsDeck : IDeck<Card>
{
    private readonly CardsDeck<Card> _deck;

    public CardsDeck()
    {
        _deck = [ ];
    }

    public CardsDeck(IEnumerable<Card> cards)
    {
        _deck = [.. cards];
    }

    #region Properties

    public Card this[int index]
    {
        get => _deck[index];
        set => _deck[index] = value;
    }

    Card IReadOnlyList<Card>.this[int index] => _deck[index];

    public Card? Top => _deck.Top;

    public int Count => _deck.Count;

    bool ICollection<Card>.IsReadOnly => ((ICollection<Card>)_deck).IsReadOnly;
    #endregion

    #region Methods

    public static CardsDeck FullDeck(bool excludeJokers = false) =>
        [.. Card.AllCards(excludeJokers)];

    #region IDeck<Card> Methods
    public void Add(Card item) =>
        _deck.Add(item);

    public void Add(params Card[ ]? cards) =>
        _deck.Add(cards);

    public void Add(IEnumerable<Card> cards) =>
        _deck.Add(cards);

    public void Clear() => _deck.Clear();

    public bool Contains(Card item) => _deck.Contains(item);

    public void CopyTo(Card[ ] array, int arrayIndex) => _deck.CopyTo(array, arrayIndex);

    public IEnumerator<Card> GetEnumerator() => _deck.GetEnumerator();

    public int IndexOf(Card item) => _deck.IndexOf(item);

    public void Insert(int index, Card item) => _deck.Insert(index, item);

    public Card Pop() => _deck.Pop();

    public void Push(Card card) => _deck.Push(card);

    public void Push(params Card[ ]? cards)
    {
        if (cards != null)
        {
            Push(cards.AsEnumerable());
        }
    }

    public void Push(IEnumerable<Card> cards)
    {
        foreach (var card in cards)
        {
            Push(card);
        }
    }

    public bool Remove(Card item) => _deck.Remove(item);

    public void RemoveAt(int index) => _deck.RemoveAt(index);

    public void Shuffle() => _deck.Shuffle();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_deck).GetEnumerator();
    public IReadonlyDeck<Card> AsReadonly() => new ReadonlyDeck<Card>(this);
    #endregion
    #endregion
}
