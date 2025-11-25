using System.Collections;

namespace Deck;

public sealed class CardsDeck<TCard> : IDeck<TCard>
    where TCard : struct
{
    #region Fields

    private readonly List<TCard> _cards;

    private readonly Random _rand = new();
    #endregion

    #region Properties

    public TCard this[int index]
    {
        get => _cards[ReversedIndex(index)];
        set => _cards[ReversedIndex(index)] = value;
    }

    public TCard? Top => Count == 0
        ? null
        : _cards[^1];

    public int Count => _cards.Count;

    bool ICollection<TCard>.IsReadOnly => false;
    #endregion

    #region Ctors

    public CardsDeck()
    {
        _cards = [ ];
    }

    public CardsDeck(IEnumerable<TCard> cards)
    {
        _cards = cards is CardsDeck<TCard> deck
            ? [.. deck._cards]
            : [.. cards.Reverse()];
    }
    #endregion

    #region Methods

    public void Add(TCard card) =>
        _cards.Insert(0, card);


    public void Add(params TCard[ ]? cards)
    {
        if (cards != null)
        {
            Add(cards.AsEnumerable());
        }
    }

    public void Add(IEnumerable<TCard> cards) => _cards.InsertRange(0, cards);

    public void Clear() =>
        _cards.Clear();

    public bool Contains(TCard card) =>
        _cards.Contains(card);

    public void CopyTo(TCard[ ] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);

        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException(
                "Destination array was not long enough. " +
                    "Check the destination index, length, and the array's lower bounds.",
                nameof(array)
            );
        }

        for (int i = 0; i < Count; i++)
        {
            array[i + arrayIndex] = this[i];
        }
    }

    public IEnumerator<TCard> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    public int IndexOf(TCard card) =>
        ReversedIndex(_cards.LastIndexOf(card));

    public void Insert(int index, TCard card) =>
        _cards.Insert(ReversedIndex(index), card);

    public TCard Pop()
    {
        if (Count == 0)
        {
            throw new InvalidOperationException("Deck is empty");
        }

        var card = Top!;
        _cards.RemoveAt(Count - 1);

        return card.Value;
    }

    // We are listing the cards in reverse, so pushing "adds" and adding "pushes"
    public void Push(TCard card) => _cards.Add(card);

    public void Push(params TCard[ ]? cards)
    {
        if (cards != null)
        {
            Push(cards.AsEnumerable());
        }
    }

    public void Push(IEnumerable<TCard> cards)
    {
        foreach (var card in cards)
        {
            Push(card);
        }
    }

    public bool Remove(TCard card)
    {
        int lastIndex = _cards.LastIndexOf(card);

        if (lastIndex == -1)
        {
            return false;
        }

        _cards.RemoveAt(lastIndex);
        return true;
    }

    public void RemoveAt(int index) =>
        _cards.RemoveAt(ReversedIndex(index));

    public void Shuffle()
    {
        if (_cards.Count == 0)
        {
            return;
        }

        var tempList = new LinkedList<TCard>(_cards);
        _cards.Clear();
        var node = tempList.First!;

        while (tempList.Count > 0)
        {
            int next = _rand.Next(tempList.Count);

            for (int i = 0; i < next; i++)
            {
                node = node.Next ?? tempList.First!;
            }

            _cards.Add(node.Value);
            node = node.Next ?? tempList.First!;

            tempList.Remove(node.Previous ?? tempList.Last!);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public IReadonlyDeck<TCard> AsReadonly() => new ReadonlyDeck<TCard>(this);

    private int ReversedIndex(int index) =>
        Math.Max(Count - index - 1, 0);
    #endregion
}

public sealed class ReadonlyDeck<TCard> : IReadonlyDeck<TCard>
    where TCard : struct
{
    private readonly IDeck<TCard> _deck;

    public ReadonlyDeck(IDeck<TCard> deck)
    {
        _deck = deck;
    }

    public TCard this[int index] => _deck[index];

    public TCard? Top => _deck.Top;

    public int Count => _deck.Count;

    public IEnumerator<TCard> GetEnumerator() => _deck.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
